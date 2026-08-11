using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record CreateApprovalWorkflowCommand(ApprovalWorkflowRequest Workflow, Guid CreatedBy)
    : IRequest<Result<CreateApprovalWorkflowResponse>>;

public class CreateApprovalWorkflowCommandValidator : AbstractValidator<CreateApprovalWorkflowCommand>
{
    public CreateApprovalWorkflowCommandValidator()
    {
        RuleFor(x => x.CreatedBy).NotEmpty();
        RuleFor(x => x.Workflow).SetValidator(new ApprovalWorkflowRequestValidator());
    }
}

public record CreateApprovalWorkflowResponse(Guid Id);

internal sealed class CreateApprovalWorkflowCommandHandler(IApprovalWorkflowRepository approvalWorkflowRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateApprovalWorkflowCommand, Result<CreateApprovalWorkflowResponse>>
{
    public async Task<Result<CreateApprovalWorkflowResponse>> Handle(CreateApprovalWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = CreateWorkflow(request.Workflow, Guid.NewGuid(), request.CreatedBy);
        await approvalWorkflowRepository.AddAsync(workflow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new CreateApprovalWorkflowResponse(workflow.Id));
    }

    internal static ApprovalWorkflow CreateWorkflow(ApprovalWorkflowRequest request, Guid workflowId, Guid createdBy)
    {
        var steps = request.Steps.Select(step => ApprovalWorkflowStep.Create(
            step.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : step.Id!.Value,
            workflowId,
            step.ReceiverPostId,
            step.IsRecommendEvent,
            step.IsApprovalEvent,
            step.EventStatus.Trim(),
            step.EventButtonText.Trim(),
            step.SequenceNo));

        return ApprovalWorkflow.Create(
            workflowId,
            request.Category.Trim(),
            request.LeaveTypeId,
            request.OrganizationUnitTypeId,
            request.SalaryGradeId,
            request.PostId,
            request.MinimumDays,
            request.MaximumDays,
            createdBy,
            steps);
    }
}

internal sealed class ApprovalWorkflowRequestValidator : AbstractValidator<ApprovalWorkflowRequest>
{
    public ApprovalWorkflowRequestValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(20);
        RuleFor(x => x.MinimumDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaximumDays).GreaterThanOrEqualTo(x => x.MinimumDays);
        RuleFor(x => x.Steps).NotEmpty();
        RuleForEach(x => x.Steps).SetValidator(new ApprovalWorkflowStepRequestValidator());
        RuleFor(x => x.Steps)
            .Must(steps => steps.Select(x => x.SequenceNo).Distinct().Count() == steps.Count)
            .WithMessage("Each workflow step must have a unique sequence number.");
    }
}

internal sealed class ApprovalWorkflowStepRequestValidator : AbstractValidator<ApprovalWorkflowStepRequest>
{
    public ApprovalWorkflowStepRequestValidator()
    {
        RuleFor(x => x.ReceiverPostId).NotEmpty();
        RuleFor(x => x.EventStatus).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EventButtonText).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SequenceNo).GreaterThan(0);
    }
}
