using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record UpdateApprovalWorkflowCommand(Guid Id, ApprovalWorkflowRequest Workflow, Guid ModifiedBy)
    : IRequest<Result<string>>;

public class UpdateApprovalWorkflowCommandValidator : AbstractValidator<UpdateApprovalWorkflowCommand>
{
    public UpdateApprovalWorkflowCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
        RuleFor(x => x.Workflow).SetValidator(new ApprovalWorkflowRequestValidator());
    }
}


internal sealed class UpdateApprovalWorkflowCommandHandler(IApprovalWorkflowRepository approvalWorkflowRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateApprovalWorkflowCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateApprovalWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = await approvalWorkflowRepository.GetByIdAsync(request.Id);
        if (workflow is null)
        {
            return new RecordNotFoundError("Approval workflow not found.");
        }

        workflow.Update(
            request.Workflow.Category.Trim(),
            request.Workflow.LeaveTypeId,
            request.Workflow.OrganizationUnitTypeId,
            request.Workflow.SalaryGradeId,
            request.Workflow.PostId,
            request.Workflow.MinimumDays,
            request.Workflow.MaximumDays,
            request.ModifiedBy);

        var steps = request.Workflow.Steps.Select(step => ApprovalWorkflowStep.Create(
            step.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : step.Id!.Value,
            workflow.Id,
            step.ReceiverPostId,
            step.IsRecommendEvent,
            step.IsApprovalEvent,
            step.EventStatus.Trim(),
            step.EventButtonText.Trim(),
            step.SequenceNo));
        workflow.ReplaceSteps(steps);

        await approvalWorkflowRepository.UpdateAsync(workflow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Approval workflow updated successfully.");
    }
}
