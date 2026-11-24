using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Cgt;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Grt.Queries;

#region --- QUERY ---
public record GetGrtQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<GrtResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGrtQueryValidator : AbstractValidator<GetGrtQuery>
{
    public GetGrtQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGrtQueryHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IGroupSurveyRepository groupSurveyRepository) : IRequestHandler<GetGrtQuery, Result<GrtResponse>>
{
    public async Task<Result<GrtResponse>> Handle(GetGrtQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("GRT is only for Group Surveys.");

        // The survey must be loaded with its Credit Officers for the access check.
        var withAssigned = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (!await SurveyAccess.CanViewAsync(context, withAssigned, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var grt = await context.TblGroupGrts.AsNoTracking()
            .Include(g => g.TblGroupGrtComponents)
            .FirstOrDefaultAsync(g => g.SurveyId == survey.Id, cancellationToken);

        var isOpen = survey.Status == SurveyStatuses.Approved;
        var cgtCompleted = await context.TblGroupCgts.AsNoTracking()
            .AnyAsync(c => c.SurveyId == survey.Id && c.Status == CgtStatuses.Completed, cancellationToken);

        var response = new GrtResponse
        {
            SurveyId = survey.Id,
            IsOpen = isOpen,
            CgtCompleted = cgtCompleted,
            IsSaved = grt is not null,
            Status = grt?.Status ?? GrtStatuses.InProgress,
            ProposedArea = survey.ProposedArea,
            BranchName = survey.BranchOrganizationUnitId.HasValue
                ? await context.TblOrganizationUnits.AsNoTracking()
                    .Where(o => o.Id == survey.BranchOrganizationUnitId).Select(o => o.Name).FirstOrDefaultAsync(cancellationToken)
                : null,
            GrtDate = grt?.GrtDate,
            CompletedOn = grt?.CompletedOn
        };

        // Only the Head of the branch writes, once the CGT is Completed, until the GRT is Completed.
        response.CanEdit = isOpen && cgtCompleted
            && survey.BranchOrganizationUnitId.HasValue
            && (grt is null || grt.Status != GrtStatuses.Completed)
            && await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value);

        response.Components = GrtComponents.All.Select(c =>
        {
            var saved = grt?.TblGroupGrtComponents.FirstOrDefault(x => x.ComponentNo == c.No);
            return new GrtComponentItem { ComponentNo = c.No, Title = c.Title, Points = c.Points, Verified = saved?.Verified, Remarks = saved?.Remarks };
        }).ToList();

        if (grt is not null)
        {
            response.Assessment = new GrtAssessment
            {
                CgtSuccessfullyCompleted = grt.CgtSuccessfullyCompleted,
                GroupDiscipline = grt.GroupDiscipline,
                ParticipationOfMembers = grt.ParticipationOfMembers,
                LeadershipQuality = grt.LeadershipQuality,
                UnderstandingOfLoanPolicies = grt.UnderstandingOfLoanPolicies,
                KnowledgeOfJointLiability = grt.KnowledgeOfJointLiability,
                FinancialLiteracy = grt.FinancialLiteracy,
                ClientProtectionAwareness = grt.ClientProtectionAwareness,
                MeetingConduct = grt.MeetingConduct,
                LoanUtilizationAwareness = grt.LoanUtilizationAwareness,
                GrievanceAwareness = grt.GrievanceAwareness,
                OverallRecommendation = grt.OverallRecommendation
            };

            var info = await groupSurveyRepository.GetSignOffAsync(grt.ConductedByEmployeeId);
            if (info is not null)
                response.ConductedBy = new GrtConductedBy { Name = info.EmployeeName, Designation = info.Designation, Pin = info.Pin };
        }

        return Result.Ok(response);
    }
}
#endregion
