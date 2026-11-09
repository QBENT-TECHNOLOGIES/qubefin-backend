using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
public record CancelTaskRequest(string? Remarks);

public record CancelTaskCommand(Guid UserId, Guid JobId, Guid TaskId, CancelTaskRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CancelTaskCommandValidator : AbstractValidator<CancelTaskCommand>
{
    public CancelTaskCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.TaskId).NotEmpty().WithMessage("Task Id is required.");
        RuleFor(v => v.Request.Remarks).NotEmpty().WithMessage("A reason is required.");
        RuleFor(v => v.Request.Remarks).MaximumLength(1000).WithMessage("The reason can be at most 1000 characters.");
    }
}
#endregion

#region --- HANDLER ---
// Any Pending task can be cancelled, whether it was assigned on the web or self-added on the mobile app
internal sealed class CancelTaskCommandHandler(JobTaskCloser taskCloser) : IRequestHandler<CancelTaskCommand, Result<string>>
{
    public Task<Result<string>> Handle(CancelTaskCommand command, CancellationToken cancellationToken) =>
        taskCloser.CloseAsync(command.UserId, command.JobId, command.TaskId, command.Request.Remarks!,
            JobTaskStatus.Cancelled, JobHistoryAction.TaskCancelled, false, cancellationToken);
}
#endregion
