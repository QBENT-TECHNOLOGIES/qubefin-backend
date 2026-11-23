using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyApprovals.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyApprovals.Queries;

#region --- QUERY ---
public record GetSurveyApprovalQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<SurveyApprovalResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetSurveyApprovalQueryValidator : AbstractValidator<GetSurveyApprovalQuery>
{
    public GetSurveyApprovalQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyApprovalQueryHandler(
    QubeFinDataContext context,
    ISurveyApprovalRepository approvalRepository) : IRequestHandler<GetSurveyApprovalQuery, Result<SurveyApprovalResponse>>
{
    public async Task<Result<SurveyApprovalResponse>> Handle(GetSurveyApprovalQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var response = new SurveyApprovalResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            CanDecide = survey.Status == SurveyStatuses.PendingApproval
                && await approvalRepository.IsManagingDirectorAsync(request.EmployeeId)
        };

        // After a Recheck the survey starts again, so an old decision is not shown. (The note is shown by the evaluation card.)
        if (survey.Status == SurveyStatuses.Approved || survey.Status == SurveyStatuses.Rejected || survey.Status == SurveyStatuses.BranchCreated)
        {
            var latest = await approvalRepository.GetLatestAsync(request.SurveyId);
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

        return response;
    }
}
#endregion
