using System.Text.Json.Serialization;

namespace QubeFin.Tracking.Application.Enums;

// Values match the web models (features/tracking/models/job.ts) and are sent as plain strings.

public enum TaskType
{
    GroupVisit,
    MemberVisit,
    OdFollowUp,
    LeadSourcing,
    Survey
}

public enum JobStatus
{
    Assigned,
    InProgress,
    Completed,
    Delayed,
    Cancelled
}

public enum JobTaskStatus
{
    Pending,
    Completed,
    Cancelled,
    Transferred,
    Rejected
}

public enum TaskOrigin
{
    Assigned,
    SelfAdded
}

public enum TaskTargetKind
{
    Member,
    OdMember,
    GroupCentre,
    Branch,
    Custom
}

public enum TaskPriority
{
    VeryHigh,
    High,
    Medium,
    Low
}

public enum JobHistoryAction
{
    Created,
    Transferred,
    TasksTransferredOut,
    TasksTransferredIn,
    TasksAdded,
    TaskSelfAdded,
    TaskCompleted,
    TaskCancelled,
    TaskRejected,
    DateExtended,
    BecameDelayed,
    Completed,
    Cancelled
}

// Days past due, both ends included (utils/dpd.ts)
public enum DpdRange
{
    Days1To30,
    Days31To60,
    Days61To90,
    Over90
}

public enum SortDirection
{
    [JsonStringEnumMemberName("asc")] Asc,
    [JsonStringEnumMemberName("desc")] Desc
}
