using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyEvaluations.Queries;

#region --- QUERY ---
public record GetSurveyEvaluationsQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<SurveyEvaluationsResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetSurveyEvaluationsQueryValidator : AbstractValidator<GetSurveyEvaluationsQuery>
{
    public GetSurveyEvaluationsQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyEvaluationsQueryHandler(
    QubeFinDataContext context,
    ISurveyEvaluationRepository evaluationRepository,
    ISurveyCommitteeRepository committeeRepository,
    ISurveyApprovalRepository approvalRepository) : IRequestHandler<GetSurveyEvaluationsQuery, Result<SurveyEvaluationsResponse>>
{
    public async Task<Result<SurveyEvaluationsResponse>> Handle(GetSurveyEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var isCommitteeLead = await committeeRepository.IsActiveLeadAsync(request.EmployeeId);
        var isEvaluator = await committeeRepository.IsActiveEvaluatorAsync(request.EmployeeId);
        var requiredEmployeeIds = await committeeRepository.GetRequiredEvaluatorEmployeeIdsAsync();

        var activeEvaluations = await evaluationRepository.GetActiveBySurveyIdAsync(request.SurveyId);
        var memberEvaluations = activeEvaluations.Where(e => !e.IsFinal).ToList();
        var finalEvaluation = activeEvaluations.FirstOrDefault(e => e.IsFinal);

        // Who has evaluated and who is still to do it (only members who are required now count).
        var doneEmployeeIds = memberEvaluations.Select(e => e.EmployeeId).Where(requiredEmployeeIds.Contains).Distinct().ToList();
        var pendingEmployeeIds = requiredEmployeeIds.Except(doneEmployeeIds).ToList();

        var isOpen = SurveyEvaluationRules.IsOpenForEvaluation(survey.Status);

        var response = new SurveyEvaluationsResponse
        {
            SurveyId = survey.Id,
            Status = survey.Status,
            IsCommitteeLead = isCommitteeLead,
            CanEvaluate = isEvaluator && isOpen && finalEvaluation is null,
            CanGiveFinal = isCommitteeLead && isOpen && finalEvaluation is null && pendingEmployeeIds.Count == 0,
            RequiredCount = requiredEmployeeIds.Count,
            DoneCount = doneEmployeeIds.Count,
            MyEvaluation = memberEvaluations.Where(e => e.EmployeeId == request.EmployeeId).Select(ToItem).FirstOrDefault(),
            FinalEvaluation = finalEvaluation is null ? null : ToItem(finalEvaluation)
        };

        // The Committee Lead sees everything. A member sees only his own evaluation (above).
        if (isCommitteeLead)
        {
            response.Evaluations = memberEvaluations.Select(ToItem).ToList();
            response.PendingMembers = await GetEmployeeNamesAsync(pendingEmployeeIds, cancellationToken);
        }

        // After a Recheck the survey is back in Draft: the surveyors need to know what to fix.
        if (survey.Status == SurveyStatuses.Draft)
        {
            // The recheck can come from the Committee Lead or from the Managing Director: show the latest one.
            var leadRecheck = await evaluationRepository.GetLatestArchivedRecheckAsync(request.SurveyId);
            var mdRecheck = await approvalRepository.GetLatestRecheckAsync(request.SurveyId);
            if (mdRecheck is not null && (leadRecheck is null || mdRecheck.ActionOn > leadRecheck.EvaluationDate))
            {
                response.RecheckComment = mdRecheck.Comments;
                response.RecheckBy = "Managing Director";
            }
            else if (leadRecheck is not null)
            {
                response.RecheckComment = leadRecheck.Comments;
                response.RecheckBy = "Committee Lead";
            }
        }

        return response;
    }

    private async Task<List<string>> GetEmployeeNamesAsync(List<Guid> employeeIds, CancellationToken cancellationToken)
    {
        if (employeeIds.Count == 0)
            return new List<string>();

        return await context.TblEmployees.AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .OrderBy(e => e.FullName)
            .Select(e => e.FullName + " (" + e.Code + ")")
            .ToListAsync(cancellationToken);
    }

    private static EvaluationItem ToItem(TblSurveyCommitteeEvaluation evaluation)
    {
        return new EvaluationItem
        {
            EmployeeName = evaluation.Employee.FullName + " (" + evaluation.Employee.Code + ")",
            Decision = evaluation.Status,
            Comments = evaluation.Comments,
            EvaluationDate = evaluation.EvaluationDate,
            IsFinal = evaluation.IsFinal
        };
    }
}
#endregion
