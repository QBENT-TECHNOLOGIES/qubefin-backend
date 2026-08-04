using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowEventByIdQuery(Guid Id) : IRequest<Result<GetApprovalWorkflowEventByIdResponse>>;

public record GetApprovalWorkflowEventByIdResponse(ApprovalWorkflowEvent Event);

internal sealed class GetApprovalWorkflowEventByIdQueryHandler(IApprovalWorkflowEventRepository approvalWorkflowEventRepository)
    : IRequestHandler<GetApprovalWorkflowEventByIdQuery, Result<GetApprovalWorkflowEventByIdResponse>>
{
    public async Task<Result<GetApprovalWorkflowEventByIdResponse>> Handle(GetApprovalWorkflowEventByIdQuery request, CancellationToken cancellationToken)
    {
        var workflowEvent = await approvalWorkflowEventRepository.GetByIdAsync(request.Id);
        return workflowEvent is null
            ? new RecordNotFoundError("Approval workflow event not found.")
            : Result.Ok(new GetApprovalWorkflowEventByIdResponse(workflowEvent));
    }
}
