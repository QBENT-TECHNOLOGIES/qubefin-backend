using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowsQuery(string? Category) : IRequest<Result<GetApprovalWorkflowsResponse>>;

public record GetApprovalWorkflowsResponse(IEnumerable<ApprovalWorkflow> Workflows);

internal sealed class GetApprovalWorkflowsQueryHandler(IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<GetApprovalWorkflowsQuery, Result<GetApprovalWorkflowsResponse>>
{
    public async Task<Result<GetApprovalWorkflowsResponse>> Handle(GetApprovalWorkflowsQuery request, CancellationToken cancellationToken)
    {
        var workflows = string.IsNullOrWhiteSpace(request.Category)
            ? await approvalWorkflowRepository.GetAllAsync()
            : await approvalWorkflowRepository.GetByCategoryAsync(request.Category.Trim());

        return Result.Ok(new GetApprovalWorkflowsResponse(workflows));
    }
}
