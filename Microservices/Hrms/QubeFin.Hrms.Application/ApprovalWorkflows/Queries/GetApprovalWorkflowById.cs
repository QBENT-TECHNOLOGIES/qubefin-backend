using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowByIdQuery(Guid Id) : IRequest<Result<ApprovalWorkflow>>;

//public record GetApprovalWorkflowByIdResponse(ApprovalWorkflow Workflow);

internal sealed class GetApprovalWorkflowByIdQueryHandler(IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<GetApprovalWorkflowByIdQuery, Result<ApprovalWorkflow>>
{
    public async Task<Result<ApprovalWorkflow>> Handle(GetApprovalWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        var workflow = await approvalWorkflowRepository.GetByIdAsync(request.Id);
        return workflow is null
            ? new RecordNotFoundError("Approval workflow not found.")
            : Result.Ok(workflow);
    }
}
