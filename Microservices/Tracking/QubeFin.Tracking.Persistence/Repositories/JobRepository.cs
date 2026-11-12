using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Data;
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
        SqlParameter Output(string name) => new(name, SqlDbType.Int) { Direction = ParameterDirection.Output };
        SqlParameter Value(string name, object? value) => new(name, value ?? DBNull.Value);
        SqlParameter Date(string name, DateOnly? value) =>
            new(name, SqlDbType.Date) { Value = value is { } date ? date.ToDateTime(TimeOnly.MinValue) : DBNull.Value };

        var totalRecords = Output("@TotalRecords");
        var countTotal = Output("@CountTotal");
        var countAssigned = Output("@CountAssigned");
        var countInProgress = Output("@CountInProgress");
        var countDelayed = Output("@CountDelayed");
        var countCompleted = Output("@CountCompleted");

        var rows = await context.Set<JobSearchRowResult>()
            .FromSqlRaw(
                "EXEC [Tracking].[USP_SearchJobs] @UserId, @EmployeeId, @Role, @UnitId, @ScopeUnitIds, @Today, @SearchText, @Status, @OpenOnly, " +
                "@OrganisationUnitId, @AssigneeId, @DueFrom, @DueTo, @DpdMin, @DpdMax, @SortOn, @SortDescending, @PageIndex, @PageSize, " +
                "@TotalRecords OUTPUT, @CountTotal OUTPUT, @CountAssigned OUTPUT, @CountInProgress OUTPUT, @CountDelayed OUTPUT, @CountCompleted OUTPUT",
                Value("@UserId", viewer.UserId),
                Value("@EmployeeId", viewer.EmployeeId),
                new SqlParameter("@Role", SqlDbType.NVarChar, 100) { Value = viewer.Role },
                Value("@UnitId", viewer.UnitId),
                // NULL scope = an Admin, who sees every job
                new SqlParameter("@ScopeUnitIds", SqlDbType.NVarChar, -1)
                {
                    Value = viewer.Role == TrackingRole.Admin ? DBNull.Value : string.Join(',', viewer.UnitIdsInScope)
                },
                Date("@Today", today),
                new SqlParameter("@SearchText", SqlDbType.NVarChar, 200) { Value = (object?)filter.SearchText ?? DBNull.Value },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = (object?)filter.Status ?? DBNull.Value },
                Value("@OpenOnly", filter.OpenOnly),
                Value("@OrganisationUnitId", filter.OrganisationUnitId),
                Value("@AssigneeId", filter.AssigneeId),
                Date("@DueFrom", filter.DueFrom),
                Date("@DueTo", filter.DueTo),
                Value("@DpdMin", filter.DpdMin),
                Value("@DpdMax", filter.DpdMax),
                new SqlParameter("@SortOn", SqlDbType.NVarChar, 50) { Value = (object?)filter.SortOn ?? DBNull.Value },
                Value("@SortDescending", filter.SortDescending),
                Value("@PageIndex", filter.PageIndex),
                Value("@PageSize", filter.PageSize),
                totalRecords, countTotal, countAssigned, countInProgress, countDelayed, countCompleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var jobs = rows.Select(r =>
        {
            var branches = BranchesOf(r.BranchesJson);
            return new JobSummaryRow(
                r.Id,
                r.JobCode,
                r.JobTitle,
                r.Status,
                r.AssigneeId,
                r.AssigneeName,
                r.AssigneeRole,
                r.UnitName,
                branches.Select(b => b.BranchId).ToList(),
                branches.Select(b => b.Name).ToList(),
                r.CreatedByName,
                r.CreatedOn,
                r.StartDate,
                r.EndDate,
                r.TaskCount,
                r.DoneCount,
                r.OpenCount,
                r.SelfAddedCount,
                TypesOf(r.TypesCsv),
                r.TransferJobRefId,
                r.TransferJobRefCode);
        }).ToList();

        var summary = new JobStatusCounts(
            (int)countTotal.Value,
            (int)countAssigned.Value,
            (int)countInProgress.Value,
            (int)countDelayed.Value,
            (int)countCompleted.Value);
        return new JobSearchResult(jobs, (int)totalRecords.Value, summary);
    }

    private sealed record JobBranch(Guid BranchId, string Name);
    public async Task<JobDetailRow?> GetJobAsync(TrackingViewer viewer, Guid jobId, DateOnly today, CancellationToken cancellationToken = default)
    {
        var job = (await context.Set<JobDetailResult>()
            .FromSqlRaw($"EXEC [Tracking].[USP_GetJob] {ViewerArguments}, @JobId",
                [.. ViewerParameters(viewer), new SqlParameter("@JobId", SqlDbType.UniqueIdentifier) { Value = jobId }])
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .FirstOrDefault();
        if (job is null) return null;

        var branches = BranchesOf(job.BranchesJson);
        var summary = new JobSummaryRow(
            job.Id,
            job.JobCode,
            job.JobTitle,
            JobStatusOf(job.IsCancel, job.PendingCount, job.DoneCount, job.EndDate, today),
            job.AssigneeId,
            job.AssigneeName,
            job.AssigneePostName ?? string.Empty,
            job.AssigneeUnitName ?? string.Empty,
            branches.Select(b => b.BranchId).ToList(),
            branches.Select(b => b.Name).ToList(),
            job.CreatedByName,
            job.CreatedOn,
            job.StartDate,
            job.EndDate,
            job.PendingCount + job.DoneCount,
            job.DoneCount,
            job.PendingCount,
            job.SelfAddedCount,
            TypesOf(job.TypesCsv),
            job.TransferJobRefId,
            job.TransferJobRefCode);
        var creator = job.CreatorIsSuperAdmin
            ? new TrackingPosition(true, TrackingRole.Admin, null)
            : new TrackingPosition(false, job.CreatorPostName, job.CreatorUnitId);

        var tasks = await context.Set<JobTaskResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetJobTasks] @JobId = {jobId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var taskRows = tasks
            .OrderBy(t => t.TaskAddedOn).ThenBy(t => t.TaskNo)
            .Select(t =>
            {
                var status = TaskStatusOf(t.Status, t.IsCompleted, t.TransferTaskId, t.IsTaskCancel);
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
                    t.CoOrdinatedByName,
                    t.TargetKind,
                    t.OriginRefId,
                    t.TargetName,
                    GeoCoordinates.ToGeoPoint(t.TargetLocation),
                    t.BranchId,
                    t.BranchName,
                    t.RadiusMeters,
                    status,
                    t.IsSelfAdded == true,
                    t.TaskAddedBy,
                    t.AddedByName,
                    t.TaskAddedOn,
                    t.ArrivedOn,
                    GeoCoordinates.ToGeoPoint(t.ArriveLocation),
                    t.ArriveDistanceMeters,
                    t.TaskCompletedOn,
                    t.CompletedByName,
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
                    t.TransferredToJobId,
                    t.TransferredToJobCode,
                    t.TaskCancelReason);
            })
            .ToList();

        return new JobDetailRow(
            summary,
            job.JobDescription,
            job.CreatedBy,
            creator,
            job.AssigneeId,
            new TrackingPosition(false, job.AssigneePostName, job.AssigneeUnitId),
            taskRows);
    }
    public async Task<List<JobHistoryRow>?> GetJobHistoryAsync(TrackingViewer viewer, Guid jobId, CancellationToken cancellationToken = default)
    {
        var isVisible = new SqlParameter("@IsVisible", SqlDbType.Bit) { Direction = ParameterDirection.Output };
        var rows = await context.Set<JobHistoryResult>()
            .FromSqlRaw($"EXEC [Tracking].[USP_GetJobHistory] {ViewerArguments}, @JobId, @IsVisible OUTPUT",
                [.. ViewerParameters(viewer), new SqlParameter("@JobId", SqlDbType.UniqueIdentifier) { Value = jobId }, isVisible])
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        if (isVisible.Value is not true) return null;

        return rows.Select(h => new JobHistoryRow(
            h.Id,
            h.JobId,
            h.Actions,
            h.PerformedByName,
            h.PerformedAt,
            h.FromAssigneeName,
            h.ToAssigneeName,
            (h.TaskNosCsv ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            h.RelatedJobId,
            h.RelatedJobCode,
            h.OldCompleteBy,
            h.NewCompleteBy,
            h.Remarks)).ToList();
    }

    public async Task AddHistoryAsync(TblJobHistory history, CancellationToken cancellationToken = default) =>
        await context.TblJobHistories.AddAsync(history, cancellationToken);
    public async Task<List<JobTaskPointRow>> GetTaskPointsAsync(TrackingViewer viewer, Guid? organisationUnitId, Guid? assigneeId, bool pendingOnly, DateOnly today, CancellationToken cancellationToken = default)
    {
        var rows = await context.Set<JobTaskPointResult>()
            .FromSqlRaw($"EXEC [Tracking].[USP_GetTaskPoints] {ViewerArguments}, @OrganisationUnitId, @AssigneeId, @Today",
                [
                    .. ViewerParameters(viewer),
                    new SqlParameter("@OrganisationUnitId", SqlDbType.UniqueIdentifier) { Value = (object?)organisationUnitId ?? DBNull.Value },
                    new SqlParameter("@AssigneeId", SqlDbType.UniqueIdentifier) { Value = (object?)assigneeId ?? DBNull.Value },
                    new SqlParameter("@Today", SqlDbType.Date) { Value = today.ToDateTime(TimeOnly.MinValue) }
                ])
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new { Row = r, Status = TaskStatusOf(r.Status, r.IsCompleted, r.TransferTaskId, r.IsTaskCancel) })
            .Where(x => x.Status == Pending || (!pendingOnly && x.Status == Completed))
            .OrderBy(x => x.Row.JobCode).ThenBy(x => x.Row.TaskNo)
            .Select(x => new JobTaskPointRow(
                x.Row.JobId,
                x.Row.JobCode,
                x.Row.JobTitle,
                x.Row.JobStatus,
                x.Row.JobStartDate,
                x.Row.JobEndDate,
                x.Row.AssigneeId,
                x.Row.AssigneeName,
                x.Row.TaskId,
                x.Row.TaskNo,
                x.Row.TaskTitle,
                x.Row.TaskType,
                x.Row.Priority,
                x.Status,
                x.Row.TargetKind,
                x.Row.OriginRefId,
                x.Row.BranchId,
                GeoCoordinates.ToGeoPoint(x.Row.TargetLocation),
                x.Status == Completed && !x.Row.IsLocationMatched))
            .ToList();
    }
    public async Task<List<PendingPlaceTask>> GetPendingTasksAtAsync(IEnumerable<Guid> originRefIds, CancellationToken cancellationToken = default)
    {
        var ids = originRefIds.Distinct().ToList();
        if (ids.Count == 0) return [];

        return (await context.Set<PendingPlaceTaskResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetPendingTasksAt] @OriginRefIds = {string.Join(',', ids)}")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .Select(t => new PendingPlaceTask(t.TaskType, t.OriginRefId, t.JobId, t.JobCode, t.AssignTo, t.AssigneeName, t.StartDate, t.EndDate))
            .ToList();
    }
    public async Task<string?> GetLastJobCodeAsync(string prefix, CancellationToken cancellationToken = default) =>
        (await context.Database
            .SqlQuery<string>($"EXEC [Tracking].[USP_GetLastJobCode] @Prefix = {prefix}")
            .ToListAsync(cancellationToken))
            .FirstOrDefault();
    public async Task<string?> GetLastTaskNoAsync(string prefix, CancellationToken cancellationToken = default) =>
        (await context.Database
            .SqlQuery<string>($"EXEC [Tracking].[USP_GetLastTaskNo] @Prefix = {prefix}")
            .ToListAsync(cancellationToken))
            .FirstOrDefault();

    public async Task AddAsync(TblJob job, CancellationToken cancellationToken = default) =>
        await context.TblJobs.AddAsync(job, cancellationToken);

    public async Task AddTasksAsync(IEnumerable<TblJobTask> tasks, CancellationToken cancellationToken = default) =>
        await context.TblJobTasks.AddRangeAsync(tasks, cancellationToken);
    public async Task<TblJob?> GetJobForUpdateAsync(Guid jobId, CancellationToken cancellationToken = default) =>
        await context.TblJobs
            .Include(j => j.TblJobTasks)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
    private const string ViewerArguments = "@UserId, @EmployeeId, @Role, @UnitId, @ScopeUnitIds";

    private static SqlParameter[] ViewerParameters(TrackingViewer viewer) =>
    [
        new("@UserId", SqlDbType.UniqueIdentifier) { Value = viewer.UserId },
        new("@EmployeeId", SqlDbType.UniqueIdentifier) { Value = (object?)viewer.EmployeeId ?? DBNull.Value },
        new("@Role", SqlDbType.NVarChar, 100) { Value = viewer.Role },
        new("@UnitId", SqlDbType.UniqueIdentifier) { Value = (object?)viewer.UnitId ?? DBNull.Value },
        new("@ScopeUnitIds", SqlDbType.NVarChar, -1)
        {
            Value = viewer.Role == TrackingRole.Admin ? DBNull.Value : string.Join(',', viewer.UnitIdsInScope)
        }
    ];

    private static List<JobBranch> BranchesOf(string? json) =>
        string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<JobBranch>>(json) ?? [];

    private static List<string> TypesOf(string? csv) =>
        (csv ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    public static bool IsPending(TblJobTask task) => TaskStatusOf(task) == Pending;

    public static string StatusOf(TblJobTask task) => TaskStatusOf(task);

    private static string TaskStatusOf(TblJobTask task) =>
        TaskStatusOf(task.Status, task.IsCompleted, task.TransferTaskId, task.IsTaskCancel);

    private static string TaskStatusOf(string? status, bool isCompleted, Guid? transferTaskId, bool isTaskCancel)
    {
        if (!string.IsNullOrWhiteSpace(status)) return status;
        if (isCompleted) return Completed;
        if (transferTaskId.HasValue) return Transferred;
        if (isTaskCancel) return Cancelled;
        return Pending;
    }
    private static string JobStatusOf(bool isCancel, int pendingCount, int doneCount, DateOnly? endDate, DateOnly today)
    {
        if (isCancel || (pendingCount == 0 && doneCount == 0)) return Cancelled;
        if (pendingCount == 0) return Completed;
        if (endDate.HasValue && today > endDate.Value) return Delayed;
        if (doneCount > 0) return InProgress;
        return Assigned;
    }
}
