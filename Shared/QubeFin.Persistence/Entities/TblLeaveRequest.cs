using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid LeaveTypeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Reason { get; set; }

    public string? Address { get; set; }

    public int? TotalDays { get; set; }

    public string? EnclosedDocName { get; set; }

    public string? EnclosedDocNo { get; set; }

    public Guid? LeavePrayerId { get; set; }

    public int LeaveYear { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public bool IsSubmitted { get; set; }

    public DateTime? SubmittedOn { get; set; }

    public Guid? SubmittedBy { get; set; }

    public DateTime? ApprovedOrRejectedOn { get; set; }

    public Guid? ApprovedOrRejectedBy { get; set; }

    public string? RejectedReason { get; set; }

    public string? FitnessReportAttachment { get; set; }

    public DateTime? FitnessReportUploadOn { get; set; }

    public bool IsFitnessReportApproved { get; set; }

    public Guid? FitnessReportApprovedBy { get; set; }

    public DateTime? FitnessReportApprovedOn { get; set; }
}
