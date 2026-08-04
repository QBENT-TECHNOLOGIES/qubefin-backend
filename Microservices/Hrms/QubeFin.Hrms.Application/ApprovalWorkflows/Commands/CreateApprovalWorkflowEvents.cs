using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record CreateApprovalWorkflowEventsCommand(IReadOnlyList<ApprovalWorkflowEventRequest> Events)
    : IRequest<Result<CreateApprovalWorkflowEventsResponse>>;

public class CreateApprovalWorkflowEventsCommandValidator : AbstractValidator<CreateApprovalWorkflowEventsCommand>
{
    public CreateApprovalWorkflowEventsCommandValidator()
    {
        RuleFor(x => x.Events).NotEmpty();
        RuleForEach(x => x.Events).SetValidator(new ApprovalWorkflowEventRequestValidator());
        RuleForEach(x => x.Events).Must(x => !x.Id.HasValue || x.Id == Guid.Empty)
            .WithMessage("An ID must not be supplied when creating a workflow event.");
    }
}

public record CreateApprovalWorkflowEventsResponse(int CreatedCount);

internal sealed class CreateApprovalWorkflowEventsCommandHandler(
    IApprovalWorkflowEventRepository approvalWorkflowEventRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateApprovalWorkflowEventsCommand, Result<CreateApprovalWorkflowEventsResponse>>
{
    public async Task<Result<CreateApprovalWorkflowEventsResponse>> Handle(CreateApprovalWorkflowEventsCommand request, CancellationToken cancellationToken)
    {
        foreach (var workflowEvent in request.Events)
        {
            await approvalWorkflowEventRepository.AddAsync(CreateDomainEvent(workflowEvent));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateApprovalWorkflowEventsResponse(request.Events.Count));
    }

    internal static ApprovalWorkflowEvent CreateDomainEvent(ApprovalWorkflowEventRequest workflowEvent)
    {
        return ApprovalWorkflowEvent.Create(
            Guid.NewGuid(),
            workflowEvent.Category.Trim(),
            workflowEvent.LeaveTypeId,
            workflowEvent.OrganizationUnitTypeId,
            workflowEvent.SalaryGradeId,
            workflowEvent.PostId,
            workflowEvent.MinimumDays,
            workflowEvent.MaximumDays,
            workflowEvent.SequenceNo,
            workflowEvent.ReceiverPostId,
            workflowEvent.IsRecommendEvent,
            workflowEvent.IsApprovalEvent,
            workflowEvent.EventStatus.Trim(),
            workflowEvent.EventButtonText.Trim());
    }
}

internal sealed class ApprovalWorkflowEventRequestValidator : AbstractValidator<ApprovalWorkflowEventRequest>
{
    public ApprovalWorkflowEventRequestValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(20);
        RuleFor(x => x.MinimumDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaximumDays)
            .GreaterThanOrEqualTo(x => x.MinimumDays)
            .When(x => x.MaximumDays.HasValue);
        RuleFor(x => x.SequenceNo).GreaterThan(0);
        RuleFor(x => x.ReceiverPostId).NotEmpty();
        RuleFor(x => x.EventStatus).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EventButtonText).NotEmpty().MaximumLength(50);
    }
}
