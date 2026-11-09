using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
public record CancelJobRequest(string? Remarks);

public record CancelJobCommand(Guid UserId, Guid JobId, CancelJobRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CancelJobCommandValidator : AbstractValidator<CancelJobCommand>
{
    public CancelJobCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.Request.Remarks).NotEmpty().WithMessage("A reason is required.");
        // Tbl_Job.CancelReason holds 200 characters
        RuleFor(v => v.Request.Remarks).MaximumLength(200).WithMessage("The reason can be at most 200 characters.");
    }
}
#endregion

#region --- HANDLER ---
// Cancels the whole job: pending tasks become Cancelled with the reason; completed tasks stay on record
// (the route and allowance for them belong to whoever did them).
internal sealed class CancelJobCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelJobCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CancelJobCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var today = TrackingClock.Today();
        var detail = await jobRepository.GetJobAsync(viewer, command.JobId, today, cancellationToken);
        if (detail is null) return new RecordNotFoundError("Job not found");
        if (!JobPermissionRules.For(viewer, detail).CanCancel)
        {
            return new ForbiddenError("Only the person who created this job, or someone above them, can do this.");
        }

        var status = Enum.Parse<JobStatus>(detail.Summary.Status);
        if (status is not (JobStatus.Assigned or JobStatus.InProgress or JobStatus.Delayed))
        {
            return new ValidationError($"{detail.Summary.JobCode} is {status} and cannot be cancelled.");
        }

        var job = (await jobRepository.GetJobForUpdateAsync(command.JobId, cancellationToken))!;
        var performedAt = DateTimeOffset.Now;
        var now = performedAt.DateTime;
        var remarks = command.Request.Remarks!.Trim();

        var cancelledTasks = job.TblJobTasks.Where(JobRepository.IsPending).ToList();
        foreach (var task in cancelledTasks)
        {
            task.Status = nameof(JobTaskStatus.Cancelled);
            task.IsTaskCancel = true;
            task.TaskCancelDate = now;
            task.TaskCancelBy = viewer.UserId;
            task.TaskCancelReason = remarks;
            task.ModifiedOn = now;
        }

        job.IsCancel = true;
        job.CancelDate = today;
        job.CanceledBy = viewer.UserId;
        job.CancelReason = remarks;
        job.ModifiedBy = viewer.UserId;
        job.ModifiedOn = now;

        // Lists only the tasks this action cancelled
        await jobRepository.AddHistoryAsync(
            JobHistoryEntries.New(job.Id, JobHistoryAction.Cancelled, viewer.UserId, performedAt, cancelledTasks,
                toAssigneeId: job.AssignTo, remarks: remarks),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{job.JobCode} cancelled. {JobMessages.Tasks(cancelledTasks.Count)} cancelled with it.");
    }
}
#endregion
