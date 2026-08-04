using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowEventsQuery(string? Category)
    : IRequest<Result<GetApprovalWorkflowEventsResponse>>;

public record GetApprovalWorkflowEventsResponse(IEnumerable<ApprovalWorkflowEvent> Events);

internal sealed class GetApprovalWorkflowEventsQueryHandler(IApprovalWorkflowEventRepository approvalWorkflowEventRepository)
    : IRequestHandler<GetApprovalWorkflowEventsQuery, Result<GetApprovalWorkflowEventsResponse>>
{
    public async Task<Result<GetApprovalWorkflowEventsResponse>> Handle(GetApprovalWorkflowEventsQuery request, CancellationToken cancellationToken)
    {
        var workflowEvents = string.IsNullOrWhiteSpace(request.Category)
            ? await approvalWorkflowEventRepository.GetAllAsync()
            : await approvalWorkflowEventRepository.GetByCategoryAsync(request.Category.Trim());

        return Result.Ok(new GetApprovalWorkflowEventsResponse(workflowEvents));
    }
}
