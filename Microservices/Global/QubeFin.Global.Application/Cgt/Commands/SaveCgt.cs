using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Cgt.Commands;

#region --- COMMAND ---
public record SaveCgtCommand(Guid SurveyId, CgtRequest Cgt, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveCgtCommandValidator : AbstractValidator<SaveCgtCommand>
{
    public SaveCgtCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Cgt).NotNull().SetValidator(new CgtRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// Any Credit Officer of an Approved Group Survey saves the CGT (In Progress) or completes it. A Completed CGT is locked.
internal sealed class SaveCgtCommandHandler(
    QubeFinDataContext context,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveCgtCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveCgtCommand request, CancellationToken cancellationToken)
    {
        var input = request.Cgt;

        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("CGT is only for Group Surveys.");
        if (survey.Status != SurveyStatuses.Approved)
            return new ValidationError("The CGT can be filled in only after the Group Survey is Approved.");
        if (!survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId))
            return new ForbiddenError("Only the Credit Officers of this survey can fill in the CGT.");

        var cgt = await context.TblGroupCgts
            .Include(c => c.TblGroupCgtTopics)
            .FirstOrDefaultAsync(c => c.SurveyId == request.SurveyId, cancellationToken);
        if (cgt is not null && cgt.Status == CgtStatuses.Completed)
            return new ValidationError("This CGT is completed and cannot be changed.");

        // The CGT starts (is saved the first time) only when Community Mobilization has enough borrowers.
        if (cgt is null)
        {
            var borrowerCount = await context.TblGroupBorrowers.AsNoTracking().CountAsync(b => b.SurveyId == request.SurveyId, cancellationToken);
            if (borrowerCount < CgtRules.MinBorrowers)
                return new ValidationError($"Add at least {CgtRules.MinBorrowers} borrowers in Community Mobilization before starting the CGT. There are {borrowerCount} now.");
        }

        // Completing needs the end date and an answer (Yes / No) for every topic.
        if (input.Complete)
        {
            if (!input.EndDate.HasValue)
                return new ValidationError("Enter the CGT End Date to complete it.");
            var unanswered = CgtTopics.All
                .Where(t => input.Topics.FirstOrDefault(x => x.TopicNo == t.No)?.Covered is null)
                .Select(t => t.No)
                .ToList();
            if (unanswered.Count > 0)
                return new ValidationError($"Answer Covered Yes or No for every topic. Missing: topic {string.Join(", ", unanswered)}.");
        }

        var now = DateTime.Now;
        if (cgt is null)
        {
            cgt = new TblGroupCgt
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                CreatedBy = request.UserId,
                CreatedOn = now
            };
            await context.TblGroupCgts.AddAsync(cgt, cancellationToken);
        }
        else
        {
            cgt.LastModifiedBy = request.UserId;
            cgt.LastModifiedOn = now;
        }

        cgt.StartDate = input.StartDate;
        cgt.EndDate = input.EndDate;
        cgt.ConductedByEmployeeId = request.EmployeeId;
        cgt.Status = input.Complete ? CgtStatuses.Completed : CgtStatuses.InProgress;
        cgt.CompletedOn = input.Complete ? now : null;

        foreach (var topic in CgtTopics.All)
        {
            var given = input.Topics.FirstOrDefault(x => x.TopicNo == topic.No);
            var row = cgt.TblGroupCgtTopics.FirstOrDefault(x => x.TopicNo == topic.No);
            if (row is null)
            {
                row = new TblGroupCgtTopic { Id = Guid.NewGuid(), CgtId = cgt.Id, TopicNo = (byte)topic.No };
                cgt.TblGroupCgtTopics.Add(row);
            }
            row.Covered = given?.Covered;
            row.Remarks = string.IsNullOrWhiteSpace(given?.Remarks) ? null : given!.Remarks!.Trim();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(input.Complete ? "CGT completed successfully." : "CGT saved successfully.");
    }
}
#endregion
