using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IJobRepository
{
    Task<JobSearchResult> SearchJobsAsync(TrackingViewer viewer, JobSearchFilter filter, DateOnly today, CancellationToken cancellationToken = default);
    Task<JobDetailRow?> GetJobAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default);
    Task<List<JobHistoryRow>?> GetJobHistoryAsync(TrackingViewer viewer, Guid jobId, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(TblJobHistory history, CancellationToken cancellationToken = default);
    Task<List<JobTaskPointRow>> GetTaskPointsAsync(TrackingViewer viewer, Guid? organisationUnitId, Guid? assigneeId, bool pendingOnly, DateOnly today, CancellationToken cancellationToken = default);
    Task<List<PendingPlaceTask>> GetPendingTasksAtAsync(IEnumerable<Guid> originRefIds, CancellationToken cancellationToken = default);
    Task<string?> GetLastJobCodeAsync(string prefix, CancellationToken cancellationToken = default);
    Task<string?> GetLastTaskNoAsync(string prefix, CancellationToken cancellationToken = default);
    Task AddAsync(TblJob job, CancellationToken cancellationToken = default);
    Task AddTasksAsync(IEnumerable<TblJobTask> tasks, CancellationToken cancellationToken = default);
    Task<TblJob?> GetJobForUpdateAsync(Guid jobId, CancellationToken cancellationToken = default);
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
                || j.AssignToNavigation.FullName.Contains(text)
                || (j.CreatedByNavigation.Employee != null ? j.CreatedByNavigation.Employee.FullName : j.CreatedByNavigation.UserName).Contains(text)
                || j.TblJobTasks.Any(t => t.TaskNo.Contains(text) || (t.TargetName != null && t.TargetName.Contains(text))));
        }
        if (filter.OrganisationUnitId.HasValue)
        {
            jobs = jobs.Where(j => j.TblJobTasks.Any(t => t.BranchId == filter.OrganisationUnitId.Value));
        }
        if (filter.AssigneeId.HasValue)
        {
            jobs = jobs.Where(j => j.AssignTo == filter.AssigneeId.Value);
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
                AssigneePosition = context.TblEmployeeDesignations
                    .Where(ed => ed.EmployeeId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => new { PostName = ed.Designation.Post.Name, ed.Designation.OrganizationUnitId })
                    .FirstOrDefault()
            })
            .AsNoTracking()
            .SingleAsync(cancellationToken);
        var creator = await GetPositionAsync(job.CreatedBy, cancellationToken);

        var tasks = await context.TblJobTasks
            .Where(t => t.JobId == jobId)
            .Select(t => new
            {
                Task = t,
                CoOrdinatedByName = t.CoOrdinatedByNavigation != null ? t.CoOrdinatedByNavigation.FullName : null,
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
                    t.TaskAddedBy,
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
            new TrackingPosition(false, job.AssigneePosition?.PostName, job.AssigneePosition?.OrganizationUnitId),
            taskRows);
    }

    // Tbl_JobHistory is append-only; each row lists the tasks it mentions through Tbl_JobHistoryTasks. Newest first.
    public async Task<List<JobHistoryRow>?> GetJobHistoryAsync(TrackingViewer viewer, Guid jobId, CancellationToken cancellationToken = default)
    {
        if (!await VisibleJobs(viewer).AnyAsync(j => j.Id == jobId, cancellationToken)) return null;

        return await context.TblJobHistories
            .Where(h => h.JobId == jobId)
            .OrderByDescending(h => h.PerformedAt)
            .Select(h => new JobHistoryRow(
                h.Id,
                h.JobId,
                h.Actions,
                h.PerformedBy != null ? (h.PerformedBy.Employee != null ? h.PerformedBy.Employee.FullName : h.PerformedBy.UserName) : SystemName,
                h.PerformedAt,
                h.FromAssignee != null ? h.FromAssignee.FullName : null,
                h.ToAssignee != null ? h.ToAssignee.FullName : null,
                h.Tasks.OrderBy(t => t.TaskNo).Select(t => t.TaskNo).ToList(),
                h.RelatedJobId,
                h.RelatedJob != null ? h.RelatedJob.JobCode : null,
                h.OldCompleteBy,
                h.NewCompleteBy,
                h.Remarks))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(TblJobHistory history, CancellationToken cancellationToken = default) =>
        await context.TblJobHistories.AddAsync(history, cancellationToken);

    // Task locations of open jobs (Assigned, InProgress, Delayed) the user may see, for the duplicate check in the task picker
    public async Task<List<JobTaskPointRow>> GetTaskPointsAsync(TrackingViewer viewer, Guid? organisationUnitId, Guid? assigneeId, bool pendingOnly, DateOnly today, CancellationToken cancellationToken = default)
    {
        var jobs = VisibleJobs(viewer).Where(j => !j.IsCancel);
        if (assigneeId.HasValue)
        {
            jobs = jobs.Where(j => j.AssignTo == assigneeId.Value);
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
                AssigneeId = (Guid?)t.Job.AssignTo
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

    // Pending tasks (in jobs not cancelled) at these places, with who holds each job and its window
    public async Task<List<PendingPlaceTask>> GetPendingTasksAtAsync(IEnumerable<Guid> originRefIds, CancellationToken cancellationToken = default)
    {
        var ids = originRefIds.Distinct().ToList();
        if (ids.Count == 0) return [];

        return await context.TblJobTasks
            .Where(t => t.OriginRefId.HasValue && ids.Contains(t.OriginRefId.Value)
                && !t.Job.IsCancel
                && (t.Status == Pending || (t.Status == null && !t.IsCompleted && !t.IsTaskCancel && t.TransferTaskId == null)))
            .Select(t => new PendingPlaceTask(
                t.TaskType,
                t.OriginRefId!.Value,
                t.JobId,
                t.Job.JobCode,
                t.Job.AssignTo,
                t.Job.AssignToNavigation.FullName,
                t.Job.StartDate,
                t.Job.EndDate))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetLastJobCodeAsync(string prefix, CancellationToken cancellationToken = default) =>
        await context.TblJobs
            .Where(j => j.JobCode.StartsWith(prefix))
            .OrderByDescending(j => j.JobCode)
            .Select(j => j.JobCode)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<string?> GetLastTaskNoAsync(string prefix, CancellationToken cancellationToken = default) =>
        await context.TblJobTasks
            .Where(t => t.TaskNo.StartsWith(prefix))
            .OrderByDescending(t => t.TaskNo)
            .Select(t => t.TaskNo)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(TblJob job, CancellationToken cancellationToken = default) =>
        await context.TblJobs.AddAsync(job, cancellationToken);

    public async Task AddTasksAsync(IEnumerable<TblJobTask> tasks, CancellationToken cancellationToken = default) =>
        await context.TblJobTasks.AddRangeAsync(tasks, cancellationToken);

    // Tracked, with its tasks, for the write APIs
    public async Task<TblJob?> GetJobForUpdateAsync(Guid jobId, CancellationToken cancellationToken = default) =>
        await context.TblJobs
            .Include(j => j.TblJobTasks)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

    // Pending under the same rule as the reads: the Status column, or the flags when it is empty
    public static bool IsPending(TblJobTask task) => TaskStatusOf(task) == Pending;

    public static string StatusOf(TblJobTask task) => TaskStatusOf(task);

    private const string SystemName = "System";

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
            j.AssignToNavigation.FullName,
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
                AssigneeId = (Guid?)j.AssignTo,
                // The assignee's role and unit come from their current designation
                AssigneeRole = context.TblEmployeeDesignations
                    .Where(ed => ed.EmployeeId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Designation.Post.Name)
                    .FirstOrDefault() ?? string.Empty,
                UnitName = context.TblEmployeeDesignations
                    .Where(ed => ed.EmployeeId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Designation.OrganizationUnit.Name)
                    .FirstOrDefault() ?? string.Empty,
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
            || j.AssignTo == viewer.EmployeeId
            || context.TblEmployeeDesignations.Any(ed => ed.EmployeeId == j.AssignTo && ed.EffectiveTo == null
                && unitIdsInScope.Contains(ed.Designation.OrganizationUnitId) && postNamesBelow.Contains(ed.Designation.Post.Name))
            || (isBranchManager && context.TblEmployeeDesignations.Any(ed => ed.EmployeeId == j.AssignTo && ed.EffectiveTo == null
                && ed.Designation.OrganizationUnitId == viewer.UnitId && ed.Designation.Post.Name == TrackingRole.AssistantBranchManager)));
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
