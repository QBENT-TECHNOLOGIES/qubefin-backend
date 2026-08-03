using System;

namespace QubeFin.Persistence.Models.Hrms;

public class AttendanceRegularization
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateOnly RegularizationDate { get; private set; }
    public string Reason { get; private set; } = null!;
    public DateTime AppliedOn { get; private set; }
    public bool IsSubmit { get; private set; }
    public DateTime? SubmitOn { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsRejected { get; private set; }
    public Guid? ActivityBy { get; private set; }
    public DateTime? ActivityOn { get; private set; }
    public string? Attachment { get; private set; }

    public AttendanceRegularization()
    {
    }

    public AttendanceRegularization(Guid id, Guid employeeId, DateOnly regularizationDate, string reason, DateTime appliedOn, bool isSubmit, DateTime? submitOn, bool isApproved, bool isRejected, Guid? activityBy, DateTime? activityOn, string? attachment)
    {
        Id = id;
        EmployeeId = employeeId;
        RegularizationDate = regularizationDate;
        Reason = reason;
        AppliedOn = appliedOn;
        IsSubmit = isSubmit;
        SubmitOn = submitOn;
        IsApproved = isApproved;
        IsRejected = isRejected;
        ActivityBy = activityBy;
        ActivityOn = activityOn;
        Attachment = attachment;
    }

    public static AttendanceRegularization CreateNew(Guid id, Guid employeeId, DateOnly regularizationDate, string reason, string? attachment)
    {
        return new AttendanceRegularization(id, employeeId, regularizationDate, reason, DateTime.UtcNow, false, null, false, false, null, null, attachment);
    }
}
