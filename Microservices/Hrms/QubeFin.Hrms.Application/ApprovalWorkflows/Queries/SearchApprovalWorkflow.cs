using FluentResults;
using MediatR;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record SearchApprovalWorkflowQuery(ApprovalWorkflowSearchRequest filterParam)
    : IRequest<Result<SearchApprovalWorkflowResponse>>;

public record SearchApprovalWorkflowResponse(
    IReadOnlyList<ApprovalWorkflow> Workflows,
    int TotalRecords);

internal sealed class SearchApprovalWorkflowQueryHandler(
    IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<SearchApprovalWorkflowQuery, Result<SearchApprovalWorkflowResponse>>
{
    public async Task<Result<SearchApprovalWorkflowResponse>> Handle(
        SearchApprovalWorkflowQuery request,
        CancellationToken cancellationToken)
    {
        var workflows = (await approvalWorkflowRepository.SearchAsync(request.filterParam.category, request.filterParam.organizationUnitTypeId, request.filterParam.SortDirection, request.filterParam.SortOn, request.filterParam.PageIndex, request.filterParam.PageSize))
            .ToList();

        return Result.Ok(new SearchApprovalWorkflowResponse(workflows, workflows.Count));
    }
}
