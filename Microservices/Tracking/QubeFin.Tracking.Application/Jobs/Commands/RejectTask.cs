using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
public record RejectTaskRequest(string? Remarks);

public record RejectTaskCommand(Guid UserId, Guid JobId, Guid TaskId, RejectTaskRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class RejectTaskCommandValidator : AbstractValidator<RejectTaskCommand>
{
    public RejectTaskCommandValidator()
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
// Only the person who added the task can reject it: the manager who added it on the web, or the assignee who self-added it
internal sealed class RejectTaskCommandHandler(JobTaskCloser taskCloser) : IRequestHandler<RejectTaskCommand, Result<string>>
{
    public Task<Result<string>> Handle(RejectTaskCommand command, CancellationToken cancellationToken) =>
        taskCloser.CloseAsync(command.UserId, command.JobId, command.TaskId, command.Request.Remarks!,
            JobTaskStatus.Rejected, JobHistoryAction.TaskRejected, true, cancellationToken);
}
#endregion
