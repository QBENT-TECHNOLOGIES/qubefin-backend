using QubeFin.Persistence.Entities;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs;

// Rows for Tbl_JobHistory (append-only) with the tasks they mention in Tbl_JobHistoryTasks.
// PerformedById is the Tbl_User.Id of who did it; FromAssigneeId / ToAssigneeId are employee ids.
internal static class JobHistoryEntries
{
    public static TblJobHistory New(
        Guid jobId,
        JobHistoryAction action,
        Guid? performedById,
        DateTimeOffset performedAt,
        IEnumerable<TblJobTask>? tasks = null,
        Guid? fromAssigneeId = null,
        Guid? toAssigneeId = null,
        Guid? relatedJobId = null,
        DateOnly? oldCompleteBy = null,
        DateOnly? newCompleteBy = null,
        string? remarks = null)
    {
        var history = new TblJobHistory
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            Actions = action.ToString(),
            PerformedById = performedById,
            PerformedAt = performedAt,
            FromAssigneeId = fromAssigneeId,
            ToAssigneeId = toAssigneeId,
            RelatedJobId = relatedJobId,
            OldCompleteBy = oldCompleteBy,
            NewCompleteBy = newCompleteBy,
            Remarks = remarks
        };
        foreach (var task in tasks ?? []) history.Tasks.Add(task);
        return history;
    }

    // An action that leaves the job with no Pending task and at least one Completed finishes it in the same transaction:
    // the job is marked completed and a Completed row is written, performed by whoever did that action
    public static TblJobHistory? CompleteIfDone(TblJob job, Guid performedById, DateTimeOffset performedAt)
    {
        if (job.IsCompleted || job.IsCancel) return null;

        var statuses = job.TblJobTasks.Select(JobRepository.StatusOf).ToList();
        if (statuses.Contains(nameof(JobTaskStatus.Pending)) || !statuses.Contains(nameof(JobTaskStatus.Completed))) return null;

        job.IsCompleted = true;
        job.CompletedDate = DateOnly.FromDateTime(performedAt.DateTime);
        job.CompletedBy = performedById;
        job.ModifiedBy = performedById;
        job.ModifiedOn = performedAt.DateTime;
        return New(job.Id, JobHistoryAction.Completed, performedById, performedAt);
    }
}
