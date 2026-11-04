using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;

namespace QubeFin.Tracking.Application.Jobs.Models;

public record CreateJobRequest(
    string? JobTitle,
    string? JobDescription,
    Guid? AssigneeId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    List<NewTaskInput>? Tasks);

// One task as entered on the web; also used when adding tasks to a job
public record NewTaskInput(
    TaskType? TaskType,
    string? TaskTitle,
    string? Notes,
    TaskPriority? Priority,
    Guid? CoOrdinatedBy,
    NewTaskTarget? Target);

// For a group, member or branch the server copies the location and branch from the database (D1);
// for a Custom map point it uses targetLocation and organisationUnitId as sent
public record NewTaskTarget(
    TaskTargetKind? Kind,
    Guid? OriginRefId,
    string? TargetName,
    GeoPoint? TargetLocation,
    Guid? OrganisationUnitId);

public record SkippedTask(string Title, string Reason);

public record TaskWarning(string Title, string Reason);

public record AddTasksResponse(JobResponse Job, List<SkippedTask> Skipped, List<TaskWarning> Warnings);
