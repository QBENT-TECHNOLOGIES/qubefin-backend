using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record SearchApprovalWorkflowQuery(string? Category, Guid? OrganizationUnitTypeId)
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
        var workflows = (await approvalWorkflowRepository.SearchAsync(
                request.Category?.Trim(),
                request.OrganizationUnitTypeId))
            .ToList();

        return Result.Ok(new SearchApprovalWorkflowResponse(workflows, workflows.Count));
    }
}
