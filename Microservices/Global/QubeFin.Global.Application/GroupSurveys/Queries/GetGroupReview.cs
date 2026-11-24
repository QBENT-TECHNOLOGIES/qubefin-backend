using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Global.Application.SurveyApprovals.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Queries;

#region --- QUERY ---
public record GetGroupReviewQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<GroupReviewResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupReviewQueryValidator : AbstractValidator<GetGroupReviewQuery>
{
    public GetGroupReviewQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGroupReviewQueryHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    ISurveyApprovalRepository approvalRepository) : IRequestHandler<GetGroupReviewQuery, Result<GroupReviewResponse>>
{
    public async Task<Result<GroupReviewResponse>> Handle(GetGroupReviewQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group || survey.BranchOrganizationUnitId is null)
            return new ValidationError("This survey is not a Group Survey.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var branchId = survey.BranchOrganizationUnitId.Value;
        var response = new GroupReviewResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            CanRecommend = survey.Status == SurveyStatuses.PendingRecommendation
                && await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, branchId),
            CanApprove = survey.Status == SurveyStatuses.PendingApproval
                && await surveyRepository.IsApproverOfBranchAsync(request.EmployeeId, branchId)
        };

        // The Head's recommendation, once given (it is cleared by a Recheck).
        var groupSurvey = await context.TblGroupSurveys.AsNoTracking()
            .Include(g => g.BranchManagerEmployee)
            .FirstOrDefaultAsync(g => g.SurveyId == survey.Id, cancellationToken);
        if (groupSurvey?.BranchManagerEmployee is not null && !string.IsNullOrEmpty(groupSurvey.BranchManagerRecommendation))
        {
            response.Recommendation = new GroupRecommendationItem
            {
                EmployeeName = groupSurvey.BranchManagerEmployee.FullName + " (" + groupSurvey.BranchManagerEmployee.Code + ")",
                Recommendation = groupSurvey.BranchManagerRecommendation,
                Comments = groupSurvey.BranchManagerComments ?? string.Empty,
                ActionOn = groupSurvey.BranchManagerActionOn
            };
        }

        // The final decision of the Area Manager.
        if (survey.Status == SurveyStatuses.Approved || survey.Status == SurveyStatuses.Rejected)
        {
            var latest = await approvalRepository.GetLatestAsync(survey.Id);
            if (latest is not null)
            {
                response.Decision = new ApprovalItem
                {
                    EmployeeName = latest.AuthorityEmployee.FullName + " (" + latest.AuthorityEmployee.Code + ")",
                    Decision = ApprovalActions.ToDecision(latest.Action),
                    Comments = latest.Comments ?? string.Empty,
                    ActionOn = latest.ActionOn
                };
            }
        }

        // After a Recheck the survey is back in Draft: the Credit Officers need to know why, and from whom.
        if (survey.Status == SurveyStatuses.Draft)
        {
            var recheck = await approvalRepository.GetLatestRecheckAsync(survey.Id);
            if (recheck is not null)
            {
                response.RecheckComment = recheck.Comments;
                var headIds = await surveyRepository.GetHeadEmployeeIdsOfBranchAsync(branchId);
                response.RecheckBy = headIds.Contains(recheck.AuthorityEmployeeId) ? "Head of the branch" : "Area Manager";
            }
        }

        return response;
    }
}
#endregion
