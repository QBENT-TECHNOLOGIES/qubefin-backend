using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs;

internal sealed record TaskPlan(
    List<TblJobTask> Tasks,
    List<SkippedTask> Skipped,
    List<TaskWarning> Warnings,
    List<string> Errors,
    Dictionary<Guid, BranchCode> Branches);

// Turns tasks entered on the web into Tbl_JobTask rows for one job and assignee, checking rules D1, D2, D3, D5, D6 and D9.
// A broken rule is an error (nothing is saved); a duplicate or an untagged place is skipped with a reason.
internal sealed class JobTaskPlanner(ITrackingScopeRepository scopeRepository, IGeoTagRepository geoTagRepository, IJobRepository jobRepository)
{
    // D2: the task type follows the place
    private static readonly Dictionary<TaskTargetKind, TaskType[]> AllowedTypes = new()
    {
        [TaskTargetKind.GroupCentre] = [TaskType.GroupVisit, TaskType.Survey, TaskType.LeadSourcing],
        [TaskTargetKind.Member] = [TaskType.MemberVisit],
        [TaskTargetKind.OdMember] = [TaskType.OdFollowUp, TaskType.MemberVisit],
        [TaskTargetKind.Branch] = [TaskType.Survey],
        [TaskTargetKind.Custom] = [TaskType.LeadSourcing, TaskType.Survey]
    };

    public async Task<TaskPlan> PlanAsync(
        TrackingViewer viewer,
        TrackingOfficer assignee,
        Guid jobId,
        DateOnly windowStart,
        DateOnly windowEnd,
        IReadOnlyList<NewTaskInput> inputs,
        Guid? existingJobId,
        DateTime now,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var tasks = new List<TblJobTask>();
        var skipped = new List<SkippedTask>();
        var warnings = new List<TaskWarning>();
        var errors = new List<string>();

        var refIds = inputs.Where(i => i.Target?.Kind != TaskTargetKind.Custom && i.Target?.OriginRefId is not null).Select(i => i.Target!.OriginRefId!.Value).ToList();
        var places = await geoTagRepository.GetPlacesAsync(refIds, cancellationToken);
        var pending = await jobRepository.GetPendingTasksAtAsync(refIds, cancellationToken);
        var setting = await scopeRepository.GetTrackingSettingAsync(cancellationToken);

        var companions = new Dictionary<Guid, TrackingOfficer?>();
        foreach (var companionId in inputs.Where(i => i.CoOrdinatedBy.HasValue).Select(i => i.CoOrdinatedBy!.Value).Distinct())
        {
            companions[companionId] = await scopeRepository.GetOfficerAsync(companionId, cancellationToken);
        }

        var branchIds = inputs.Select(i => i.Target?.OrganisationUnitId).OfType<Guid>()
            .Concat(places.Values.Select(p => p.BranchId))
            .Concat(assignee.BranchIds);
        var branches = await scopeRepository.GetBranchCodesAsync(branchIds, cancellationToken);
        string BranchName(Guid id) => branches.TryGetValue(id, out var b) ? b.Name : "this branch";

        var seenKeys = new HashSet<(TaskType, Guid)>();
        foreach (var input in inputs)
        {
            var title = input.TaskTitle!.Trim();
            var type = input.TaskType!.Value;
            var target = input.Target!;
            var kind = target.Kind!.Value;

            if (!AllowedTypes[kind].Contains(type))
            {
                errors.Add($"{title}: {type} cannot be done at a {kind}.");
                continue;
            }

            // D1: a geo-tagged place takes its location and branch from the database; a map point uses what was sent
            Guid branchId;
            NetTopologySuite.Geometries.Geometry? location;
            Guid? originRefId = null;
            if (kind == TaskTargetKind.Custom)
            {
                branchId = target.OrganisationUnitId!.Value;
                location = GeoCoordinates.ToGeometry(target.TargetLocation!);
            }
            else
            {
                if (!places.TryGetValue(target.OriginRefId!.Value, out var place) || !IsSameKind(kind, place.Kind))
                {
                    errors.Add($"{title}: the place was not found.");
                    continue;
                }
                branchId = place.BranchId;
                location = place.Location;
                originRefId = place.RefId;
            }

            // D3: the assignee must work in the task's branch
            if (!assignee.BranchIds.Contains(branchId))
            {
                errors.Add($"{title}: {assignee.Name} does not work in {BranchName(branchId)}.");
                continue;
            }

            // D9: the person going along works in the task's branch (or is the user), and never holds the job
            Guid? coOrdinatedBy = null;
            if (input.CoOrdinatedBy.HasValue)
            {
                var companion = companions[input.CoOrdinatedBy.Value];
                if (input.CoOrdinatedBy == assignee.EmployeeId)
                {
                    errors.Add($"{title}: {assignee.Name} holds the job, so cannot also be the person going along.");
                    continue;
                }
                if (companion is null)
                {
                    errors.Add($"{title}: the person going along was not found.");
                    continue;
                }
                if (!companion.BranchIds.Contains(branchId) && companion.EmployeeId != viewer.EmployeeId)
                {
                    errors.Add($"{title}: {companion.Name} does not work in {BranchName(branchId)}.");
                    continue;
                }
                coOrdinatedBy = companion.EmployeeId;
            }

            // D6: only places with a verified location (every tagged place counts as verified for now)
            if (location is null)
            {
                skipped.Add(new SkippedTask(title, "Location not verified"));
                continue;
            }

            // D5: duplicates, keyed on task type + place
            if (originRefId.HasValue)
            {
                var refId = originRefId.Value;
                if (!seenKeys.Add((type, refId)))
                {
                    skipped.Add(new SkippedTask(title, "Listed twice in this request"));
                    continue;
                }
                var samePlace = pending.Where(p => p.OriginRefId == refId).ToList();
                if (existingJobId.HasValue && samePlace.Any(p => p.JobId == existingJobId && p.TaskType == type.ToString()))
                {
                    skipped.Add(new SkippedTask(title, "Already pending in this job"));
                    continue;
                }
                var samePerson = samePlace.Where(p => p.AssignTo == assignee.EmployeeId && p.JobId != existingJobId).ToList();
                var followUp = type == TaskType.OdFollowUp ? samePerson.FirstOrDefault(p => p.TaskType == nameof(TaskType.OdFollowUp)) : null;
                if (followUp is not null)
                {
                    skipped.Add(new SkippedTask(title, $"Also pending with {followUp.AssigneeName} ({followUp.JobCode})"));
                    continue;
                }
                var overlapping = samePerson.FirstOrDefault(p => p.TaskType == type.ToString() && Overlaps(p, windowStart, windowEnd, today));
                if (overlapping is not null)
                {
                    skipped.Add(new SkippedTask(title, $"Already pending with {overlapping.AssigneeName} in {overlapping.JobCode}"));
                    continue;
                }
                var someoneElse = samePlace.FirstOrDefault(p => p.AssignTo != assignee.EmployeeId && p.TaskType == type.ToString());
                if (someoneElse is not null)
                {
                    warnings.Add(new TaskWarning(title, $"Also pending with {someoneElse.AssigneeName} ({someoneElse.JobCode})"));
                }
            }

            tasks.Add(new TblJobTask
            {
                Id = Guid.NewGuid(),
                TaskNo = string.Empty,
                JobId = jobId,
                TaskType = type.ToString(),
                BranchId = branchId,
                OriginRefId = originRefId,
                TaskTitle = title,
                Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim(),
                Priority = (input.Priority ?? TaskPriority.Medium).ToString(),
                CoOrdinatedBy = coOrdinatedBy,
                TargetName = target.TargetName!.Trim(),
                TargetLocation = location,
                RadiusMeters = setting?.TaskLocationMatchInMeters,
                Status = nameof(JobTaskStatus.Pending),
                IsSelfAdded = false,
                TaskAddedBy = viewer.UserId,
                TaskAddedOn = now
            });
        }

        return new TaskPlan(tasks, skipped, warnings, errors, branches);
    }

    // Member and OdMember are both a member's home; whether they are overdue can change after the web loaded them
    private static bool IsSameKind(TaskTargetKind kind, string placeKind) => kind switch
    {
        TaskTargetKind.Member or TaskTargetKind.OdMember => placeKind is nameof(TaskTargetKind.Member) or nameof(TaskTargetKind.OdMember),
        _ => placeKind == kind.ToString()
    };

    // A job is active from its start date to its end date; a delayed job stays active until today
    private static bool Overlaps(PendingPlaceTask task, DateOnly windowStart, DateOnly windowEnd, DateOnly today)
    {
        var end = task.EndDate.HasValue && task.EndDate.Value > today ? task.EndDate.Value : today;
        return task.StartDate <= windowEnd && end >= windowStart;
    }
}
