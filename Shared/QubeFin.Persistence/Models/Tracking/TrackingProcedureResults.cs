using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Models.Tracking;

// Rows of the Tracking stored procedures that replaced the LINQ reads. The repositories map them to the Tracking models.

// Tracking.USP_GetTrackingViewer; the CSV columns are comma-separated ids
public class TrackingViewerResult
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? PostName { get; set; }
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitIdsCsv { get; set; }
    public string? BranchIdsCsv { get; set; }
}

// Tracking.USP_GetTrackingOfficer
public class TrackingOfficerResult
{
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; } = null!;
    public Guid DesignationId { get; set; }
    public string PostName { get; set; } = null!;
    public Guid UnitId { get; set; }
    public string? BranchIdsCsv { get; set; }
}

// Tracking.USP_GetTrackingBranches
public class TrackingBranchResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? AreaName { get; set; }
    public string? DivisionName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

// Tracking.USP_GetBranchCodes
public class BranchCodeResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int CodeVal { get; set; }
}

// Tracking.USP_GetJob; BranchesJson is [{ "BranchId", "Name" }] sorted by name, TypesCsv the distinct task types
public class JobDetailResult
{
    public Guid Id { get; set; }
    public string JobCode { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public bool IsCancel { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? AssigneeName { get; set; }
    public int PendingCount { get; set; }
    public int DoneCount { get; set; }
    public Guid AssigneeId { get; set; }
    public string? AssigneePostName { get; set; }
    public Guid? AssigneeUnitId { get; set; }
    public string? AssigneeUnitName { get; set; }
    public string? BranchesJson { get; set; }
    public string CreatedByName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public int SelfAddedCount { get; set; }
    public string? TypesCsv { get; set; }
    public Guid? TransferJobRefId { get; set; }
    public string? TransferJobRefCode { get; set; }
    public string? JobDescription { get; set; }
    public Guid CreatedBy { get; set; }
    public bool CreatorIsSuperAdmin { get; set; }
    public string? CreatorPostName { get; set; }
    public Guid? CreatorUnitId { get; set; }
}

// Tracking.USP_GetJobTasks
public class JobTaskResult
{
    public Guid Id { get; set; }
    public string TaskNo { get; set; } = null!;
    public Guid JobId { get; set; }
    public string TaskType { get; set; } = null!;
    public string? TaskTitle { get; set; }
    public string? Notes { get; set; }
    public string? Priority { get; set; }
    public Guid? CoOrdinatedBy { get; set; }
    public string? CoOrdinatedByName { get; set; }
    public string? TargetKind { get; set; }
    public Guid? OriginRefId { get; set; }
    public string? TargetName { get; set; }
    public Geometry? TargetLocation { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public int? RadiusMeters { get; set; }
    public string? Status { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsTaskCancel { get; set; }
    public Guid? TransferTaskId { get; set; }
    public bool? IsSelfAdded { get; set; }
    public Guid? TaskAddedBy { get; set; }
    public string? AddedByName { get; set; }
    public DateTime? TaskAddedOn { get; set; }
    public DateTime? ArrivedOn { get; set; }
    public Geometry? ArriveLocation { get; set; }
    public decimal? ArriveDistanceMeters { get; set; }
    public DateTime? TaskCompletedOn { get; set; }
    public string? CompletedByName { get; set; }
    public Geometry? CompletedLocation { get; set; }
    public decimal? CompletedDistanceMeter { get; set; }
    public bool IsLocationMatched { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsOdMemberAvailable { get; set; }
    public bool IsEmireceived { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public DateOnly? Odptpdate { get; set; }
    public string? Remarks { get; set; }
    public Guid? TransferredToJobId { get; set; }
    public string? TransferredToJobCode { get; set; }
    public string? TaskCancelReason { get; set; }
}

// Tracking.USP_GetJobHistory; TaskNosCsv is the sorted task numbers
public class JobHistoryResult
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string Actions { get; set; } = null!;
    public string PerformedByName { get; set; } = null!;
    public DateTimeOffset PerformedAt { get; set; }
    public string? FromAssigneeName { get; set; }
    public string? ToAssigneeName { get; set; }
    public string? TaskNosCsv { get; set; }
    public Guid? RelatedJobId { get; set; }
    public string? RelatedJobCode { get; set; }
    public DateOnly? OldCompleteBy { get; set; }
    public DateOnly? NewCompleteBy { get; set; }
    public string? Remarks { get; set; }
}

// Tracking.USP_GetTaskPoints
public class JobTaskPointResult
{
    public Guid JobId { get; set; }
    public string JobCode { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string JobStatus { get; set; } = null!;
    public DateOnly JobStartDate { get; set; }
    public DateOnly? JobEndDate { get; set; }
    public Guid AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public Guid TaskId { get; set; }
    public string TaskNo { get; set; } = null!;
    public string? TaskTitle { get; set; }
    public string TaskType { get; set; } = null!;
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsTaskCancel { get; set; }
    public Guid? TransferTaskId { get; set; }
    public string? TargetKind { get; set; }
    public Guid? OriginRefId { get; set; }
    public Guid BranchId { get; set; }
    public Geometry? TargetLocation { get; set; }
    public bool IsLocationMatched { get; set; }
}

// Tracking.USP_GetPendingTasksAt
public class PendingPlaceTaskResult
{
    public string TaskType { get; set; } = null!;
    public Guid OriginRefId { get; set; }
    public Guid JobId { get; set; }
    public string JobCode { get; set; } = null!;
    public Guid AssignTo { get; set; }
    public string? AssigneeName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

// Tracking.USP_GetGeoTagPoints; a group fills GroupName, a member FirstName / MiddleName / LastName
public class GeoTagPointResult
{
    public string Kind { get; set; } = null!;
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public Geometry? Location { get; set; }
    public DateTime? TaggedOn { get; set; }
    public string? TaggedByName { get; set; }
    public Guid GroupId { get; set; }
    public Guid? CreditOfficerId { get; set; }
    public string? CreditOfficerName { get; set; }
    public int? Dpd { get; set; }
    public decimal? OverdueAmount { get; set; }
    public string BranchName { get; set; } = null!;
}

// Tracking.USP_GetTrackingPlaces; a branch fills Latitude / Longitude, the others Location
public class TrackingPlaceResult
{
    public Guid RefId { get; set; }
    public string Kind { get; set; } = null!;
    public Guid BranchId { get; set; }
    public Geometry? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int Priority { get; set; }
}

// Tracking.USP_GetGeoTags; members fill FirstName / MiddleName / LastName, the others Name; branches fill Latitude / Longitude
public class GeoTagResult
{
    public Guid RefId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Code { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public Geometry? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? LocationPhoto { get; set; }
    public DateTime? TaggedOn { get; set; }
    public string? TaggedByName { get; set; }
}

// Tracking.USP_FindGeoTag
public class GeoTagTargetResult
{
    public string Kind { get; set; } = null!;
    public Guid? BranchId { get; set; }
    public Guid? OwnerDesignationId { get; set; }
    public string? OwnerPostName { get; set; }
    public Guid? OwnerUnitId { get; set; }
    public bool IsOwnedByViewer { get; set; }
}
