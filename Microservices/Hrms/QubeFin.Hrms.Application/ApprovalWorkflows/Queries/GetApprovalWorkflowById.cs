using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

public record GetApprovalWorkflowByIdQuery(Guid Id) : IRequest<Result<GetApprovalWorkflowByIdResponse>>;

public record GetApprovalWorkflowByIdResponse(ApprovalWorkflow Workflow);

internal sealed class GetApprovalWorkflowByIdQueryHandler(IApprovalWorkflowRepository approvalWorkflowRepository)
    : IRequestHandler<GetApprovalWorkflowByIdQuery, Result<GetApprovalWorkflowByIdResponse>>
{
    public async Task<Result<GetApprovalWorkflowByIdResponse>> Handle(GetApprovalWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        var workflow = await approvalWorkflowRepository.GetByIdAsync(request.Id);
        return workflow is null
            ? new RecordNotFoundError("Approval workflow not found.")
            : Result.Ok(new GetApprovalWorkflowByIdResponse(workflow));
    }
}
