using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Commands;

public record UpdateApprovalWorkflowEventsCommand(IReadOnlyList<ApprovalWorkflowEventRequest> Events)
    : IRequest<Result<UpdateApprovalWorkflowEventsResponse>>;

public class UpdateApprovalWorkflowEventsCommandValidator : AbstractValidator<UpdateApprovalWorkflowEventsCommand>
{
    public UpdateApprovalWorkflowEventsCommandValidator()
    {
        RuleFor(x => x.Events).NotEmpty();
        RuleForEach(x => x.Events).SetValidator(new ApprovalWorkflowEventRequestValidator());
        RuleFor(x => x.Events)
            .Must(events => events.Where(x => x.Id.HasValue && x.Id != Guid.Empty).Select(x => x.Id).Distinct().Count()
                == events.Count(x => x.Id.HasValue && x.Id != Guid.Empty))
            .WithMessage("Each workflow event can appear only once in an update request.");
    }
}

public record UpdateApprovalWorkflowEventsResponse(int CreatedCount, int UpdatedCount);

internal sealed class UpdateApprovalWorkflowEventsCommandHandler(
    IApprovalWorkflowEventRepository approvalWorkflowEventRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateApprovalWorkflowEventsCommand, Result<UpdateApprovalWorkflowEventsResponse>>
{
    public async Task<Result<UpdateApprovalWorkflowEventsResponse>> Handle(UpdateApprovalWorkflowEventsCommand request, CancellationToken cancellationToken)
    {
        var existingEvents = new Dictionary<Guid, QubeFin.Persistence.Models.Hrms.ApprovalWorkflowEvent>();

        foreach (var eventId in request.Events.Where(x => x.Id.HasValue && x.Id != Guid.Empty).Select(x => x.Id!.Value))
        {
            var workflowEvent = await approvalWorkflowEventRepository.GetByIdAsync(eventId);
            if (workflowEvent is null)
            {
                return new RecordNotFoundError($"Approval workflow event '{eventId}' was not found.");
            }

            existingEvents.Add(eventId, workflowEvent);
        }

        var createdCount = 0;
        var updatedCount = 0;

        foreach (var workflowEventRequest in request.Events)
        {
            if (!workflowEventRequest.Id.HasValue || workflowEventRequest.Id == Guid.Empty)
            {
                await approvalWorkflowEventRepository.AddAsync(
                    CreateApprovalWorkflowEventsCommandHandler.CreateDomainEvent(workflowEventRequest));
                createdCount++;
                continue;
            }

            var workflowEvent = existingEvents[workflowEventRequest.Id.Value];
            workflowEvent.Update(
                workflowEventRequest.Category.Trim(),
                workflowEventRequest.LeaveTypeId,
                workflowEventRequest.OrganizationUnitTypeId,
                workflowEventRequest.SalaryGradeId,
                workflowEventRequest.PostId,
                workflowEventRequest.MinimumDays,
                workflowEventRequest.MaximumDays,
                workflowEventRequest.SequenceNo,
                workflowEventRequest.ReceiverPostId,
                workflowEventRequest.IsRecommendEvent,
                workflowEventRequest.IsApprovalEvent,
                workflowEventRequest.EventStatus.Trim(),
                workflowEventRequest.EventButtonText.Trim());
            await approvalWorkflowEventRepository.UpdateAsync(workflowEvent);
            updatedCount++;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateApprovalWorkflowEventsResponse(createdCount, updatedCount));
    }
}
