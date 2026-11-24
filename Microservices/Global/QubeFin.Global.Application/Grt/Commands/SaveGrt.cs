using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Cgt;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Grt.Commands;

#region --- COMMAND ---
public record SaveGrtCommand(Guid SurveyId, GrtRequest Grt, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveGrtCommandValidator : AbstractValidator<SaveGrtCommand>
{
    public SaveGrtCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Grt).NotNull().SetValidator(new GrtRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The Head of the branch (Branch Manager) saves the GRT (In Progress) or submits it, once the CGT is Completed.
// A submitted GRT is locked. A Fail stops the process: no group can be created from the survey.
internal sealed class SaveGrtCommandHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveGrtCommand, Result<string>>
{
    private const int MaxNotificationLength = 200;

    public async Task<Result<string>> Handle(SaveGrtCommand request, CancellationToken cancellationToken)
    {
        var input = request.Grt;

        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("GRT is only for Group Surveys.");
        if (survey.Status != SurveyStatuses.Approved || survey.BranchOrganizationUnitId is null)
            return new ValidationError("The GRT can be filled in only after the Group Survey is Approved.");
        if (!await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value))
            return new ForbiddenError("Only the Head of the branch (Branch Manager) can fill in the GRT.");
        if (!await context.TblGroupCgts.AnyAsync(c => c.SurveyId == survey.Id && c.Status == CgtStatuses.Completed, cancellationToken))
            return new ValidationError("The GRT can start only after the CGT is Completed.");

        var grt = await context.TblGroupGrts
            .Include(g => g.TblGroupGrtComponents)
            .FirstOrDefaultAsync(g => g.SurveyId == request.SurveyId, cancellationToken);
        if (grt is not null && grt.Status == GrtStatuses.Completed)
            return new ValidationError("This GRT is submitted and cannot be changed.");

        var a = input.Assessment;

        // Submitting needs every component and every assessment answer.
        if (input.Submit)
        {
            var missingComponents = GrtComponents.All
                .Where(c => input.Components.FirstOrDefault(x => x.ComponentNo == c.No)?.Verified is null)
                .Select(c => c.No)
                .ToList();
            if (missingComponents.Count > 0)
                return new ValidationError($"Answer Verified Yes or No for every component. Missing: component {string.Join(", ", missingComponents)}.");

            if (a.CgtSuccessfullyCompleted is null)
                return new ValidationError("Answer 'CGT Successfully Completed' in the Final Group Assessment.");
            var missingRating = a.Ratings().FirstOrDefault(r => string.IsNullOrEmpty(r.Value));
            if (missingRating.Label is not null)
                return new ValidationError($"Answer '{missingRating.Label}' in the Final Group Assessment.");
            if (string.IsNullOrEmpty(a.OverallRecommendation))
                return new ValidationError("Choose the Overall Recommendation (Pass or Fail).");
        }

        var now = DateTime.Now;
        if (grt is null)
        {
            grt = new TblGroupGrt
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                CreatedBy = request.UserId,
                CreatedOn = now
            };
            await context.TblGroupGrts.AddAsync(grt, cancellationToken);
        }
        else
        {
            grt.LastModifiedBy = request.UserId;
            grt.LastModifiedOn = now;
        }

        grt.GrtDate = input.GrtDate;
        grt.ConductedByEmployeeId = request.EmployeeId;
        grt.Status = input.Submit ? GrtStatuses.Completed : GrtStatuses.InProgress;
        grt.CompletedOn = input.Submit ? now : null;

        grt.CgtSuccessfullyCompleted = a.CgtSuccessfullyCompleted;
        grt.GroupDiscipline = a.GroupDiscipline;
        grt.ParticipationOfMembers = a.ParticipationOfMembers;
        grt.LeadershipQuality = a.LeadershipQuality;
        grt.UnderstandingOfLoanPolicies = a.UnderstandingOfLoanPolicies;
        grt.KnowledgeOfJointLiability = a.KnowledgeOfJointLiability;
        grt.FinancialLiteracy = a.FinancialLiteracy;
        grt.ClientProtectionAwareness = a.ClientProtectionAwareness;
        grt.MeetingConduct = a.MeetingConduct;
        grt.LoanUtilizationAwareness = a.LoanUtilizationAwareness;
        grt.GrievanceAwareness = a.GrievanceAwareness;
        grt.OverallRecommendation = a.OverallRecommendation;

        foreach (var component in GrtComponents.All)
        {
            var given = input.Components.FirstOrDefault(x => x.ComponentNo == component.No);
            var row = grt.TblGroupGrtComponents.FirstOrDefault(x => x.ComponentNo == component.No);
            if (row is null)
            {
                row = new TblGroupGrtComponent { Id = Guid.NewGuid(), GrtId = grt.Id, ComponentNo = (byte)component.No };
                grt.TblGroupGrtComponents.Add(row);
            }
            row.Verified = given?.Verified;
            row.Remarks = string.IsNullOrWhiteSpace(given?.Remarks) ? null : given!.Remarks!.Trim();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // The Credit Officers of the survey are told the result.
        if (input.Submit)
        {
            var passed = a.OverallRecommendation == "Pass";
            var message = passed
                ? $"{survey.AssignmentNo} ({survey.ProposedArea}): the GRT is passed. The group can be created."
                : $"{survey.AssignmentNo} ({survey.ProposedArea}): the GRT is failed. The process stops here.";
            if (message.Length > MaxNotificationLength)
                message = message.Substring(0, MaxNotificationLength - 3) + "...";

            await notificationRepository.AddForEmployeesAsync(
                survey.TblSurveyAssigneds.Select(x => x.EmployeeId).Where(id => id != request.EmployeeId).Distinct().ToList(),
                title: passed ? "GRT Passed" : "GRT Failed",
                message: message,
                icon: passed ? "circle-check" : "x-circle",
                referenceId: survey.Id,
                referenceType: "SURVEY",
                actionUrl: "/secure/global/surveys",
                createdBy: request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(input.Submit ? "GRT submitted successfully." : "GRT saved successfully.");
    }
}
#endregion
