using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms;

public class LeaveRequest
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public DateOnly FromDate { get; private set; }
    public DateOnly ToDate { get; private set; }
    public DateTime RequestDate { get; private set; }
    public string? Reason { get; private set; }
    public string? Address { get; private set; }
    public int? TotalDays { get; private set; }
    public string? EnclosedDocName { get; private set; }
    public string? EnclosedDocNo { get; private set; }
    public Guid? LeavePrayerId { get; private set; }
    public int LeaveYear { get; private set; }
    public string CurrentStatus { get; private set; } = null!;
    public bool IsSubmitted { get; set; }
    public DateTime? SubmittedOn { get; set; }
    public Guid? SubmittedBy { get; set; }

    public LeaveRequest()
    {
        
    }
    public LeaveRequest(Guid id, Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, DateTime requestDate, string? reason, string? address,
        int? totalDays, string? enclosedDocName, string? enclosedDocNo, Guid? leavePrayerId, int leaveYear, string currentStatus, bool isSubmitted, DateTime? submittedOn, Guid? submittedBy)
    {
        Id = id;
        EmployeeId = employeeId;
        LeaveTypeId = leaveTypeId;
        FromDate = fromDate;
        ToDate = toDate;
        RequestDate = requestDate;
        Reason = reason;
        Address = address;
        TotalDays = totalDays;
        EnclosedDocName = enclosedDocName;
        EnclosedDocNo = enclosedDocNo;
        LeavePrayerId = leavePrayerId;
        LeaveYear = leaveYear;
        CurrentStatus = currentStatus;
        IsSubmitted = isSubmitted;
        SubmittedOn = submittedOn;
        SubmittedBy = submittedBy;
    }

    public void Submit(bool isSubmitted, Guid userId)
    {
        if (IsSubmitted)
        {
            IsSubmitted = isSubmitted;
            SubmittedOn = DateTime.Now;
            SubmittedBy = userId;
        }
    }
}

public class LeaveRequestResponse
{
    public Guid? Id { get; set; }
    public Guid? LeaveTypeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? LeaveType { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public int? TotalDays { get; set; }
    public string? CurrentStatus { get; set; }
    public string? Reason { get; set; }
    public string? Address { get; set; }
    public string? EnclosedDocName { get; set; }
    public string? EnclosedDocNo { get; set; }
    public bool? IsSubmitted { get; set; }
    public bool? IsCancellable { get; set; }
    public string? RejectedReason { get; set; }
    public string? ApprovalCategory { get; set; }
    public DateTime? EventDate { get; set; }
    public string? Remarks { get; set; }
    public string? SenderDesignation { get; set; }
    public string? ReceiverDesignation { get; set; }
    public string? EventCategory { get; set; }
    public string? EventStatus { get; set; }
    public string? EventButtonText { get; set; }
    public bool? IsRecommendEvent { get; set; }
    public bool? IsApprovalEvent { get; set; }
    public string? FitnessReportAttachment { get; set; }
    public bool? IsFitnessReportApproved { get; set; }
    public string? FitnessReportApprovedBy { get; set; }
}
