using FluentResults;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs;

// Closes one Pending task without doing it (cancel or reject): the task gets the new status and the closing columns,
// one history row records who closed it and why, and the job completes if no pending work is left and something was done.
// Cancel: anyone who may update the job. Reject: only the person who added the task (TaskAddedBy), whether a manager on the
// web or the assignee who self-added it on the mobile app.
internal sealed class JobTaskCloser(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<string>> CloseAsync(
        Guid userId,
        Guid jobId,
        Guid taskId,
        string remarks,
        JobTaskStatus closedStatus,
        JobHistoryAction action,
        bool onlyByAdder,
        CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(userId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var today = TrackingClock.Today();
        var detail = await jobRepository.GetJobAsync(viewer, jobId, today, cancellationToken);
        if (detail is null) return new RecordNotFoundError("Job not found");
        if (!onlyByAdder && !JobPermissionRules.For(viewer, detail).CanUpdate)
        {
            return new ForbiddenError("Only the person who created this job, someone above them, or a manager above the assignee can do this.");
        }

        var job = (await jobRepository.GetJobForUpdateAsync(jobId, cancellationToken))!;
        var task = job.TblJobTasks.FirstOrDefault(t => t.Id == taskId);
        if (task is null) return new RecordNotFoundError("Task not found");
        if (!JobRepository.IsPending(task)) return new ValidationError($"{task.TaskNo} is {JobRepository.StatusOf(task)}.");
        if (onlyByAdder && task.TaskAddedBy != viewer.UserId) return new ForbiddenError("Only the person who added this task can reject it.");

        var performedAt = DateTimeOffset.Now;
        var now = performedAt.DateTime;
        remarks = remarks.Trim();

        task.Status = closedStatus.ToString();
        task.IsTaskCancel = true;
        task.TaskCancelDate = now;
        task.TaskCancelBy = viewer.UserId;
        task.TaskCancelReason = remarks;
        task.ModifiedOn = now;
        job.ModifiedBy = viewer.UserId;
        job.ModifiedOn = now;

        await jobRepository.AddHistoryAsync(
            JobHistoryEntries.New(job.Id, action, viewer.UserId, performedAt, [task], toAssigneeId: job.AssignTo, remarks: remarks),
            cancellationToken);
        var completed = JobHistoryEntries.CompleteIfDone(job, viewer.UserId, performedAt);
        if (completed is not null) await jobRepository.AddHistoryAsync(completed, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var message = $"Task {task.TaskNo} {closedStatus.ToString().ToLowerInvariant()}.";
        return Result.Ok(completed is null ? message : $"{message} {job.JobCode} is now completed.");
    }
}
