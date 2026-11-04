using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IJobRepository
{
    Task<JobSearchResult> SearchJobsAsync(TrackingViewer viewer, JobSearchFilter filter, DateOnly today, CancellationToken cancellationToken = default);
    Task<JobDetailRow?> GetJobAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default);
    Task<List<JobHistoryRow>?> GetJobHistoryAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default);
    Task<List<JobTaskPointRow>> GetTaskPointsAsync(TrackingViewer viewer, Guid? organisationUnitId, Guid? assigneeId, bool pendingOnly, DateOnly today, CancellationToken cancellationToken = default);
}

public class JobRepository(QubeFinDataContext context) : IJobRepository
{
    // Job and task status names (D8); Tbl_JobTask.Status holds the task ones, the flags are the fallback when it is empty
    private const string Assigned = "Assigned";
    private const string InProgress = "InProgress";
    private const string Delayed = "Delayed";
    private const string Completed = "Completed";
    private const string Cancelled = "Cancelled";
    private const string Pending = "Pending";
    private const string Transferred = "Transferred";
    private const string OdFollowUp = "OdFollowUp";
    private const string MemberVisit = "MemberVisit";

    public async Task<JobSearchResult> SearchJobsAsync(TrackingViewer viewer, JobSearchFilter filter, DateOnly today, CancellationToken cancellationToken = default)
    {
        var jobs = VisibleJobs(viewer);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var text = filter.SearchText.Trim();
            jobs = jobs.Where(j => j.JobCode.Contains(text)
                || j.JobTitle.Contains(text)
                || context.TblEmployeeDesignations.Any(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null && ed.Employee.FullName.Contains(text))
                || (j.CreatedByNavigation.Employee != null ? j.CreatedByNavigation.Employee.FullName : j.CreatedByNavigation.UserName).Contains(text)
                || j.TblJobTasks.Any(t => t.TaskNo.Contains(text) || (t.TargetName != null && t.TargetName.Contains(text))));
        }
        if (filter.OrganisationUnitId.HasValue)
        {
            jobs = jobs.Where(j => j.TblJobTasks.Any(t => t.BranchId == filter.OrganisationUnitId.Value));
        }
        if (filter.AssigneeId.HasValue)
        {
            jobs = jobs.Where(j => context.TblEmployeeDesignations.Any(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null && ed.EmployeeId == filter.AssigneeId.Value));
        }
        if (filter.DueFrom.HasValue)
        {
            jobs = jobs.Where(j => j.EndDate >= filter.DueFrom.Value);
        }
        if (filter.DueTo.HasValue)
        {
            jobs = jobs.Where(j => j.EndDate <= filter.DueTo.Value);
        }
        if (filter.DpdMin.HasValue)
        {
            var dpdMin = filter.DpdMin.Value;
            var dpdMax = filter.DpdMax ?? int.MaxValue;
            // OriginRefId may be a member id or a loan id; the member's current DPD is the highest DPD of their open loans
            jobs = jobs.Where(j => j.TblJobTasks.Any(t =>
                (t.TaskType == OdFollowUp || t.TaskType == MemberVisit)
                && (t.Status == Pending || t.Status == Completed || (t.Status == null && !t.IsTaskCancel && t.TransferTaskId == null))
                && context.TblBcloans
                    .Where(l => !l.IsClosed && (l.Id == t.OriginRefId || l.BcmemberId == t.OriginRefId))
                    .Max(l => l.Dpddays) >= dpdMin
                && context.TblBcloans
                    .Where(l => !l.IsClosed && (l.Id == t.OriginRefId || l.BcmemberId == t.OriginRefId))
                    .Max(l => l.Dpddays) <= dpdMax));
        }

        var keys = (await JobKeys(jobs).ToListAsync(cancellationToken))
            .Select(k => k with { Status = JobStatusOf(k.IsCancel, k.PendingCount, k.DoneCount, k.EndDate, today) })
            .ToList();

        // Counts use every filter except status, so the top cards do not change when a status card is clicked
        var summary = new JobStatusCounts(
            keys.Count,
            keys.Count(k => k.Status == Assigned),
            keys.Count(k => k.Status == InProgress),
            keys.Count(k => k.Status == Delayed),
            keys.Count(k => k.Status == Completed));

        var filtered = filter.Status is not null
            ? keys.Where(k => k.Status == filter.Status)
            : filter.OpenOnly
                ? keys.Where(k => k.Status is Assigned or InProgress or Delayed)
                : keys;

        var sorted = (filter.SortOn?.ToLowerInvariant()) switch
        {
            "createdon" or "createdat" => filter.SortDescending ? filtered.OrderByDescending(k => k.CreatedOn) : filtered.OrderBy(k => k.CreatedOn),
            "assigneename" => filter.SortDescending ? filtered.OrderByDescending(k => k.AssigneeName) : filtered.OrderBy(k => k.AssigneeName),
            "status" => filter.SortDescending ? filtered.OrderByDescending(k => k.Status) : filtered.OrderBy(k => k.Status),
            _ => filter.SortDescending ? filtered.OrderByDescending(k => k.EndDate) : filtered.OrderBy(k => k.EndDate)
        };

        var matching = sorted.ThenBy(k => k.JobCode).ToList();
        var page = matching.Skip(filter.PageIndex * filter.PageSize).Take(filter.PageSize).ToList();
        var rows = await SummaryRowsAsync(page, cancellationToken);

        return new JobSearchResult(rows, matching.Count, summary);
    }

    public async Task<JobDetailRow?> GetJobAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default)
    {
        var key = await JobKeys(VisibleJobs(viewer).Where(j => j.Id == jobId)).FirstOrDefaultAsync(cancellationToken);
        if (key is null) return null;
        key = key with { Status = JobStatusOf(key.IsCancel, key.PendingCount, key.DoneCount, key.EndDate, today) };
        var summary = (await SummaryRowsAsync([key], cancellationToken)).Single();

        var job = await context.TblJobs
            .Where(j => j.Id == jobId)
            .Select(j => new
            {
                j.JobDescription,
                j.CreatedBy,
                j.AssignTo,
                AssigneePost = j.AssignToNavigation.Post.Name,
                AssigneeUnitId = j.AssignToNavigation.OrganizationUnitId
            })
            .AsNoTracking()
            .SingleAsync(cancellationToken);
        var creator = await GetPositionAsync(job.CreatedBy, cancellationToken);

        var tasks = await context.TblJobTasks
            .Where(t => t.JobId == jobId)
            .Select(t => new
            {
                Task = t,
                CoOrdinatedByName = context.TblEmployeeDesignations
                    .Where(ed => ed.DesignationId == t.CoOrdinatedBy && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Employee.FullName)
                    .FirstOrDefault(),
                BranchName = t.Branch.Name,
                AddedByName = t.TaskAddedByNavigation != null
                    ? (t.TaskAddedByNavigation.Employee != null ? t.TaskAddedByNavigation.Employee.FullName : t.TaskAddedByNavigation.UserName)
                    : null,
                CompletedByName = t.TaskCompletedByNavigation != null
                    ? (t.TaskCompletedByNavigation.Employee != null ? t.TaskCompletedByNavigation.Employee.FullName : t.TaskCompletedByNavigation.UserName)
                    : null,
                TransferredTo = context.TblJobTasks
                    .Where(n => n.Id == t.TransferTaskId)
                    .Select(n => new { n.JobId, n.Job.JobCode })
                    .FirstOrDefault()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var targetKinds = await TargetKindsAsync(tasks.Select(t => t.Task.OriginRefId), cancellationToken);

        var taskRows = tasks
            .OrderBy(t => t.Task.TaskAddedOn).ThenBy(t => t.Task.TaskNo)
            .Select(x =>
            {
                var t = x.Task;
                var status = TaskStatusOf(t);
                var isLate = summary.EndDate.HasValue && (status == Completed
                    ? t.TaskCompletedOn.HasValue && DateOnly.FromDateTime(t.TaskCompletedOn.Value) > summary.EndDate.Value
                    : status == Pending && today > summary.EndDate.Value);
                return new JobTaskRow(
                    t.Id,
                    t.TaskNo,
                    t.JobId,
                    t.TaskType,
                    t.TaskTitle,
                    t.Notes,
                    t.Priority,
                    t.CoOrdinatedBy,
                    x.CoOrdinatedByName,
                    t.OriginRefId.HasValue ? targetKinds.GetValueOrDefault(t.OriginRefId.Value) : "Custom",
                    t.OriginRefId,
                    t.TargetName,
                    GeoCoordinates.ToGeoPoint(t.TargetLocation),
                    t.BranchId,
                    x.BranchName,
                    t.RadiusMeters,
                    status,
                    t.IsSelfAdded == true,
                    x.AddedByName,
                    t.TaskAddedOn,
                    t.ArrivedOn,
                    GeoCoordinates.ToGeoPoint(t.ArriveLocation),
                    t.ArriveDistanceMeters,
                    t.TaskCompletedOn,
                    x.CompletedByName,
                    GeoCoordinates.ToGeoPoint(t.CompletedLocation),
                    t.CompletedDistanceMeter,
                    t.IsLocationMatched,
                    t.PhotoUrl,
                    t.IsOdMemberAvailable,
                    t.IsEmireceived,
                    t.ReceivedAmount,
                    t.Odptpdate,
                    t.Remarks,
                    isLate,
                    x.TransferredTo?.JobId,
                    x.TransferredTo?.JobCode,
                    t.TaskCancelReason);
            })
            .ToList();

        return new JobDetailRow(
            summary,
            job.JobDescription,
            job.CreatedBy,
            creator,
            job.AssignTo,
            new TrackingPosition(false, job.AssigneePost, job.AssigneeUnitId),
            taskRows);
    }

    // There is no history table, so the trail is rebuilt from the "by" and "on" columns of Tbl_Job and Tbl_JobTask.
    // A whole-job transfer and a date extension overwrite their columns, so they cannot be shown.
    public async Task<List<JobHistoryRow>?> GetJobHistoryAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default)
    {
        var job = await VisibleJobs(viewer)
            .Where(j => j.Id == jobId)
            .Select(j => new
            {
                j.Id,
                j.CreatedOn,
                CreatedByName = j.CreatedByNavigation.Employee != null ? j.CreatedByNavigation.Employee.FullName : j.CreatedByNavigation.UserName,
                AssigneeName = context.TblEmployeeDesignations
                    .Where(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Employee.FullName)
                    .FirstOrDefault(),
                j.EndDate,
                j.IsCancel,
                j.CanceledBy,
                CanceledByName = j.CanceledByNavigation != null
                    ? (j.CanceledByNavigation.Employee != null ? j.CanceledByNavigation.Employee.FullName : j.CanceledByNavigation.UserName)
                    : null,
                j.CancelDate,
                j.CancelReason,
                j.TransferJobRefId,
                TransferJobRefCode = j.TransferJobRef != null ? j.TransferJobRef.JobCode : null,
                TransferFromAssigneeName = j.TransferJobRef != null
                    ? context.TblEmployeeDesignations
                        .Where(ed => ed.DesignationId == j.TransferJobRef.AssignTo && ed.EffectiveTo == null)
                        .OrderByDescending(ed => ed.EffectiveFrom)
                        .Select(ed => ed.Employee.FullName)
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (job is null) return null;

        var tasks = await context.TblJobTasks
            .Where(t => t.JobId == jobId)
            .Select(t => new
            {
                Task = t,
                AddedByName = t.TaskAddedByNavigation != null
                    ? (t.TaskAddedByNavigation.Employee != null ? t.TaskAddedByNavigation.Employee.FullName : t.TaskAddedByNavigation.UserName)
                    : null,
                CompletedByName = t.TaskCompletedByNavigation != null
                    ? (t.TaskCompletedByNavigation.Employee != null ? t.TaskCompletedByNavigation.Employee.FullName : t.TaskCompletedByNavigation.UserName)
                    : null,
                CancelByName = t.TaskCancelByNavigation != null
                    ? (t.TaskCancelByNavigation.Employee != null ? t.TaskCancelByNavigation.Employee.FullName : t.TaskCancelByNavigation.UserName)
                    : null,
                TransferredTo = context.TblJobTasks
                    .Where(n => n.Id == t.TransferTaskId)
                    .Select(n => new
                    {
                        n.JobId,
                        n.Job.JobCode,
                        n.Job.CreatedOn,
                        CreatedByName = n.Job.CreatedByNavigation.Employee != null ? n.Job.CreatedByNavigation.Employee.FullName : n.Job.CreatedByNavigation.UserName,
                        AssigneeName = context.TblEmployeeDesignations
                            .Where(ed => ed.DesignationId == n.Job.AssignTo && ed.EffectiveTo == null)
                            .OrderByDescending(ed => ed.EffectiveFrom)
                            .Select(ed => ed.Employee.FullName)
                            .FirstOrDefault()
                    })
                    .FirstOrDefault()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var entries = new List<JobHistoryRow>();
        void Add(string action, string? performedByName, DateTime? performedOn, IEnumerable<string> taskNos,
            string? fromAssigneeName = null, string? toAssigneeName = null, Guid? relatedJobId = null, string? relatedJobCode = null, string? remarks = null)
        {
            var taskNoList = taskNos.OrderBy(n => n).ToList();
            entries.Add(new JobHistoryRow(
                HistoryId(jobId, action, performedOn, taskNoList),
                jobId,
                action,
                performedByName ?? SystemName,
                performedOn,
                fromAssigneeName,
                toAssigneeName,
                taskNoList,
                relatedJobId,
                relatedJobCode,
                remarks));
        }

        // Tasks put in by a manager: the ones added with the job make up "Created", later batches are "TasksAdded"
        var managerBatches = tasks
            .Where(t => t.Task.IsSelfAdded != true)
            .GroupBy(t => new { t.Task.TaskAddedOn, t.AddedByName })
            .ToList();
        var createdTaskNos = managerBatches
            .Where(b => b.Key.TaskAddedOn is null || job.CreatedOn is null || b.Key.TaskAddedOn <= job.CreatedOn.Value.AddMinutes(1))
            .SelectMany(b => b.Select(t => t.Task.TaskNo))
            .ToList();

        Add("Created", job.CreatedByName, job.CreatedOn, createdTaskNos, toAssigneeName: job.AssigneeName);
        if (job.TransferJobRefId.HasValue)
        {
            Add("TasksTransferredIn", job.CreatedByName, job.CreatedOn, createdTaskNos,
                job.TransferFromAssigneeName, job.AssigneeName, job.TransferJobRefId, job.TransferJobRefCode);
        }
        foreach (var batch in managerBatches.Where(b => b.Key.TaskAddedOn.HasValue && job.CreatedOn.HasValue && b.Key.TaskAddedOn > job.CreatedOn.Value.AddMinutes(1)))
        {
            Add("TasksAdded", batch.Key.AddedByName, batch.Key.TaskAddedOn, batch.Select(t => t.Task.TaskNo));
        }

        foreach (var t in tasks.Where(t => t.Task.IsSelfAdded == true))
        {
            Add("TaskSelfAdded", t.AddedByName, t.Task.TaskAddedOn, [t.Task.TaskNo]);
        }

        foreach (var t in tasks.Where(t => TaskStatusOf(t.Task) == Completed))
        {
            Add("TaskCompleted", t.CompletedByName, t.Task.TaskCompletedOn, [t.Task.TaskNo], remarks: t.Task.Remarks);
        }

        foreach (var t in tasks.Where(t => TaskStatusOf(t.Task) is Cancelled or Rejected))
        {
            // Tasks closed by cancelling the whole job are covered by the job's "Cancelled" entry
            var closedWithJob = job.IsCancel && t.Task.TaskCancelBy == job.CanceledBy && t.Task.TaskCancelReason == job.CancelReason;
            if (closedWithJob) continue;
            var action = TaskStatusOf(t.Task) == Rejected ? "TaskRejected" : "TaskCancelled";
            Add(action, t.CancelByName, t.Task.TaskCancelDate, [t.Task.TaskNo], remarks: t.Task.TaskCancelReason);
        }

        foreach (var moved in tasks.Where(t => t.TransferredTo is not null).GroupBy(t => t.TransferredTo!.JobId))
        {
            var target = moved.First().TransferredTo!;
            Add("TasksTransferredOut", target.CreatedByName, target.CreatedOn, moved.Select(t => t.Task.TaskNo),
                job.AssigneeName, target.AssigneeName, target.JobId, target.JobCode);
        }

        // Nightly rule: the job passed its end date with pending tasks
        var pendingCount = tasks.Count(t => TaskStatusOf(t.Task) == Pending);
        if (job.EndDate.HasValue && !job.IsCancel)
        {
            var lastDay = job.EndDate.Value;
            var completedLate = tasks.Any(t => TaskStatusOf(t.Task) == Completed && t.Task.TaskCompletedOn.HasValue && DateOnly.FromDateTime(t.Task.TaskCompletedOn.Value) > lastDay);
            if ((pendingCount > 0 && today > lastDay) || completedLate)
            {
                Add("BecameDelayed", null, lastDay.AddDays(1).ToDateTime(TimeOnly.MinValue), []);
            }
        }

        var doneTasks = tasks.Where(t => TaskStatusOf(t.Task) == Completed).ToList();
        if (!job.IsCancel && pendingCount == 0 && doneTasks.Count > 0)
        {
            var last = doneTasks.OrderByDescending(t => t.Task.TaskCompletedOn).First();
            Add("Completed", last.CompletedByName, last.Task.TaskCompletedOn, []);
        }

        if (job.IsCancel)
        {
            Add("Cancelled", job.CanceledByName, job.CancelDate?.ToDateTime(TimeOnly.MinValue), [], remarks: job.CancelReason);
        }

        // Newest first
        return entries
            .OrderByDescending(e => e.PerformedOn ?? DateTime.MinValue)
            .ThenByDescending(e => e.Action == "Created" ? 0 : 1)
            .ToList();
    }

    // Task locations of open jobs (Assigned, InProgress, Delayed) the user may see, for the duplicate check in the task picker
    public async Task<List<JobTaskPointRow>> GetTaskPointsAsync(TrackingViewer viewer, Guid? organisationUnitId, Guid? assigneeId, bool pendingOnly, DateOnly today, CancellationToken cancellationToken = default)
    {
        var jobs = VisibleJobs(viewer).Where(j => !j.IsCancel);
        if (assigneeId.HasValue)
        {
            jobs = jobs.Where(j => context.TblEmployeeDesignations.Any(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null && ed.EmployeeId == assigneeId.Value));
        }
        if (organisationUnitId.HasValue)
        {
            jobs = jobs.Where(j => j.TblJobTasks.Any(t => t.BranchId == organisationUnitId.Value));
        }

        var openJobs = (await JobKeys(jobs).ToListAsync(cancellationToken))
            .Select(k => k with { Status = JobStatusOf(k.IsCancel, k.PendingCount, k.DoneCount, k.EndDate, today) })
            .Where(k => k.Status is Assigned or InProgress or Delayed)
            .ToDictionary(k => k.Id);
        if (openJobs.Count == 0) return [];

        var openJobIds = openJobs.Keys.ToList();
        var tasks = await context.TblJobTasks
            .Where(t => openJobIds.Contains(t.JobId))
            .Where(t => !organisationUnitId.HasValue || t.BranchId == organisationUnitId.Value)
            .Select(t => new
            {
                Task = t,
                t.Job.JobTitle,
                t.Job.StartDate,
                AssigneeId = context.TblEmployeeDesignations
                    .Where(ed => ed.DesignationId == t.Job.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => (Guid?)ed.EmployeeId)
                    .FirstOrDefault()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var points = tasks
            .Select(x => new { x.Task, x.JobTitle, x.StartDate, x.AssigneeId, Status = TaskStatusOf(x.Task) })
            .Where(x => x.Status == Pending || (!pendingOnly && x.Status == Completed))
            .ToList();
        var targetKinds = await TargetKindsAsync(points.Select(p => p.Task.OriginRefId), cancellationToken);

        return points
            .OrderBy(p => openJobs[p.Task.JobId].JobCode).ThenBy(p => p.Task.TaskNo)
            .Select(p =>
            {
                var job = openJobs[p.Task.JobId];
                return new JobTaskPointRow(
                    job.Id,
                    job.JobCode,
                    p.JobTitle,
                    job.Status,
                    p.StartDate,
                    job.EndDate,
                    p.AssigneeId,
                    job.AssigneeName,
                    p.Task.Id,
                    p.Task.TaskNo,
                    p.Task.TaskTitle,
                    p.Task.TaskType,
                    p.Task.Priority,
                    p.Status,
                    p.Task.OriginRefId.HasValue ? targetKinds.GetValueOrDefault(p.Task.OriginRefId.Value) : "Custom",
                    p.Task.OriginRefId,
                    p.Task.BranchId,
                    GeoCoordinates.ToGeoPoint(p.Task.TargetLocation),
                    p.Status == Completed && !p.Task.IsLocationMatched);
            })
            .ToList();
    }

    private const string SystemName = "System";
    private const string Rejected = "Rejected";

    // Entries are rebuilt on every read, so their id is derived from what they describe to stay stable
    private static Guid HistoryId(Guid jobId, string action, DateTime? performedOn, IEnumerable<string> taskNos)
    {
        var key = $"{jobId}|{action}|{performedOn:O}|{string.Join(',', taskNos)}";
        return new Guid(System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(key)));
    }

    private sealed record JobKey(
        Guid Id,
        string JobCode,
        bool IsCancel,
        DateOnly? EndDate,
        DateTime? CreatedOn,
        string? AssigneeName,
        int PendingCount,
        int DoneCount,
        string Status);

    private IQueryable<JobKey> JobKeys(IQueryable<TblJob> jobs) =>
        jobs.Select(j => new JobKey(
            j.Id,
            j.JobCode,
            j.IsCancel,
            j.EndDate,
            j.CreatedOn,
            context.TblEmployeeDesignations
                .Where(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null)
                .OrderByDescending(ed => ed.EffectiveFrom)
                .Select(ed => ed.Employee.FullName)
                .FirstOrDefault(),
            j.TblJobTasks.Count(t => t.Status == Pending || (t.Status == null && !t.IsCompleted && !t.IsTaskCancel && t.TransferTaskId == null)),
            j.TblJobTasks.Count(t => t.Status == Completed || (t.Status == null && t.IsCompleted)),
            string.Empty));

    private async Task<List<JobSummaryRow>> SummaryRowsAsync(List<JobKey> keys, CancellationToken cancellationToken)
    {
        var ids = keys.Select(k => k.Id).ToList();
        var details = await context.TblJobs
            .Where(j => ids.Contains(j.Id))
            .Select(j => new
            {
                j.Id,
                j.JobTitle,
                AssigneeId = context.TblEmployeeDesignations
                    .Where(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => (Guid?)ed.EmployeeId)
                    .FirstOrDefault(),
                AssigneeRole = j.AssignToNavigation.Post.Name,
                UnitName = j.AssignToNavigation.OrganizationUnit.Name,
                CreatedByName = j.CreatedByNavigation.Employee != null ? j.CreatedByNavigation.Employee.FullName : j.CreatedByNavigation.UserName,
                j.StartDate,
                j.TransferJobRefId,
                TransferJobRefCode = j.TransferJobRef != null ? j.TransferJobRef.JobCode : null,
                SelfAddedCount = j.TblJobTasks.Count(t => t.IsSelfAdded == true),
                Types = j.TblJobTasks.Select(t => t.TaskType).Distinct().ToList(),
                Branches = j.TblJobTasks.Select(t => new { t.BranchId, t.Branch.Name }).Distinct().ToList()
            })
            .AsNoTracking()
            .ToDictionaryAsync(j => j.Id, cancellationToken);

        return keys
            .Where(k => details.ContainsKey(k.Id))
            .Select(k =>
            {
                var d = details[k.Id];
                var branches = d.Branches.OrderBy(b => b.Name).ToList();
                return new JobSummaryRow(
                    k.Id,
                    k.JobCode,
                    d.JobTitle,
                    k.Status,
                    d.AssigneeId,
                    k.AssigneeName,
                    d.AssigneeRole,
                    d.UnitName,
                    branches.Select(b => b.BranchId).ToList(),
                    branches.Select(b => b.Name).ToList(),
                    d.CreatedByName,
                    k.CreatedOn,
                    d.StartDate,
                    k.EndDate,
                    k.PendingCount + k.DoneCount,
                    k.DoneCount,
                    k.PendingCount,
                    d.SelfAddedCount,
                    d.Types,
                    d.TransferJobRefId,
                    d.TransferJobRefCode);
            })
            .ToList();
    }

    // D4 "see a job": Admin sees all; everyone sees jobs they created or hold, plus jobs in their scope held by someone below them,
    // and a Branch Manager also sees jobs held by the Assistant Branch Manager of their branch
    private IQueryable<TblJob> VisibleJobs(TrackingViewer viewer)
    {
        var jobs = context.TblJobs.AsNoTracking();
        if (viewer.Role == TrackingRole.Admin) return jobs;

        var postNamesBelow = TrackingRole.PostNamesBelow(viewer.Role).ToList();
        var unitIdsInScope = viewer.UnitIdsInScope;
        var isBranchManager = viewer.Role == TrackingRole.BranchManager;

        return jobs.Where(j => j.CreatedBy == viewer.UserId
            || context.TblEmployeeDesignations.Any(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null && ed.EmployeeId == viewer.EmployeeId)
            || (unitIdsInScope.Contains(j.AssignToNavigation.OrganizationUnitId) && postNamesBelow.Contains(j.AssignToNavigation.Post.Name))
            || (isBranchManager && j.AssignToNavigation.OrganizationUnitId == viewer.UnitId && j.AssignToNavigation.Post.Name == TrackingRole.AssistantBranchManager));
    }

    private async Task<TrackingPosition> GetPositionAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await context.TblUsers
            .Where(u => u.Id == userId)
            .Select(u => new { u.IsSuperAdmin, u.EmployeeId })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) return new TrackingPosition(false, null, null);
        if (user.IsSuperAdmin) return new TrackingPosition(true, TrackingRole.Admin, null);

        var position = await context.TblEmployeeDesignations
            .Where(ed => ed.EmployeeId == user.EmployeeId && ed.EffectiveTo == null)
            .OrderByDescending(ed => ed.EffectiveFrom)
            .Select(ed => new { PostName = ed.Designation.Post.Name, ed.Designation.OrganizationUnitId })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return new TrackingPosition(false, position?.PostName, position?.OrganizationUnitId);
    }

    // OriginRefId may point to a group, a member, a loan or a branch; null means a point picked on the map
    private async Task<Dictionary<Guid, string>> TargetKindsAsync(IEnumerable<Guid?> originRefIds, CancellationToken cancellationToken)
    {
        var ids = originRefIds.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var kinds = new Dictionary<Guid, string>();
        if (ids.Count == 0) return kinds;

        foreach (var id in await context.TblBcgroups.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync(cancellationToken))
        {
            kinds[id] = "GroupCentre";
        }
        foreach (var member in await context.TblBcmembers.Where(m => ids.Contains(m.Id)).Select(m => new { m.Id, m.IsOverdueMember }).ToListAsync(cancellationToken))
        {
            kinds[member.Id] = member.IsOverdueMember ? "OdMember" : "Member";
        }
        foreach (var id in await context.TblBcloans.Where(l => ids.Contains(l.Id)).Select(l => l.Id).ToListAsync(cancellationToken))
        {
            kinds[id] = "OdMember";
        }
        foreach (var id in await context.TblOrganizationUnits.Where(o => ids.Contains(o.Id)).Select(o => o.Id).ToListAsync(cancellationToken))
        {
            kinds[id] = "Branch";
        }
        return kinds;
    }

    private static string TaskStatusOf(TblJobTask task)
    {
        if (!string.IsNullOrWhiteSpace(task.Status)) return task.Status;
        if (task.IsCompleted) return Completed;
        if (task.TransferTaskId.HasValue) return Transferred;
        if (task.IsTaskCancel) return Cancelled;
        return Pending;
    }

    // D8, first match wins
    private static string JobStatusOf(bool isCancel, int pendingCount, int doneCount, DateOnly? endDate, DateOnly today)
    {
        if (isCancel || (pendingCount == 0 && doneCount == 0)) return Cancelled;
        if (pendingCount == 0) return Completed;
        if (endDate.HasValue && today > endDate.Value) return Delayed;
        if (doneCount > 0) return InProgress;
        return Assigned;
    }
}
