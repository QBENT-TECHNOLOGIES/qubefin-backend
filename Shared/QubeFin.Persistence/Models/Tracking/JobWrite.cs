using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Models.Tracking;

// An employee's current designation and the branches it covers
public record TrackingOfficer(
    Guid EmployeeId,
    string Name,
    Guid DesignationId,
    string PostName,
    Guid UnitId,
    IReadOnlyList<Guid> BranchIds);

// A geo-tagged place a task can point at (group, member or branch)
public record TrackingPlace(Guid RefId, string Kind, Guid BranchId, Geometry? Location);

// A pending task at a geo-tagged place, for the duplicate rule (D5)
public record PendingPlaceTask(
    string TaskType,
    Guid OriginRefId,
    Guid JobId,
    string JobCode,
    Guid AssignTo,
    string? AssigneeName,
    DateOnly StartDate,
    DateOnly? EndDate);

public record BranchCode(Guid Id, string Name, int CodeVal);
