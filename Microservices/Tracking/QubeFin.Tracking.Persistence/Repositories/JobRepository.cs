using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IJobRepository
{
    Task<JobSearchResult> SearchJobsAsync(TrackingViewer viewer, JobSearchFilter filter, DateOnly today, CancellationToken cancellationToken = default);
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

        var keys = await jobs
            .Select(j => new
            {
                j.Id,
                j.JobCode,
                j.IsCancel,
                j.EndDate,
                j.CreatedOn,
                AssigneeName = context.TblEmployeeDesignations
                    .Where(ed => ed.DesignationId == j.AssignTo && ed.EffectiveTo == null)
                    .OrderByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Employee.FullName)
                    .FirstOrDefault(),
                PendingCount = j.TblJobTasks.Count(t => t.Status == Pending || (t.Status == null && !t.IsCompleted && !t.IsTaskCancel && t.TransferTaskId == null)),
                DoneCount = j.TblJobTasks.Count(t => t.Status == Completed || (t.Status == null && t.IsCompleted))
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var withStatus = keys
            .Select(k => new { k.Id, k.JobCode, k.EndDate, k.CreatedOn, k.AssigneeName, k.PendingCount, k.DoneCount, Status = JobStatusOf(k.IsCancel, k.PendingCount, k.DoneCount, k.EndDate, today) })
            .ToList();

        // Counts use every filter except status, so the top cards do not change when a status card is clicked
        var summary = new JobStatusCounts(
            withStatus.Count,
            withStatus.Count(k => k.Status == Assigned),
            withStatus.Count(k => k.Status == InProgress),
            withStatus.Count(k => k.Status == Delayed),
            withStatus.Count(k => k.Status == Completed));

        var filtered = filter.Status is not null
            ? withStatus.Where(k => k.Status == filter.Status)
            : filter.OpenOnly
                ? withStatus.Where(k => k.Status is Assigned or InProgress or Delayed)
                : withStatus;

        var sorted = (filter.SortOn?.ToLowerInvariant()) switch
        {
            "createdon" or "createdat" => filter.SortDescending ? filtered.OrderByDescending(k => k.CreatedOn) : filtered.OrderBy(k => k.CreatedOn),
            "assigneename" => filter.SortDescending ? filtered.OrderByDescending(k => k.AssigneeName) : filtered.OrderBy(k => k.AssigneeName),
            "status" => filter.SortDescending ? filtered.OrderByDescending(k => k.Status) : filtered.OrderBy(k => k.Status),
            _ => filter.SortDescending ? filtered.OrderByDescending(k => k.EndDate) : filtered.OrderBy(k => k.EndDate)
        };

        var matching = sorted.ThenBy(k => k.JobCode).ToList();
        var page = matching.Skip(filter.PageIndex * filter.PageSize).Take(filter.PageSize).ToList();
        var pageIds = page.Select(k => k.Id).ToList();

        var details = await context.TblJobs
            .Where(j => pageIds.Contains(j.Id))
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

        var rows = page
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

        return new JobSearchResult(rows, matching.Count, summary);
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
