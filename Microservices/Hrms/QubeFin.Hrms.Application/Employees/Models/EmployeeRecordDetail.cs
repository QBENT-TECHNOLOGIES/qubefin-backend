namespace QubeFin.Hrms.Application.Employees.Models
{
    /// <summary>
    /// Read-only view of a single record for the employee records console. Unlike the
    /// per-module detail procedures it carries no approve/reject affordances, so it is
    /// not scoped to the designation of the caller.
    /// </summary>
    public class EmployeeRecordDetail
    {
        public Guid Id { get; set; }
        public string RecordType { get; set; } = string.Empty;

        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? OrganizationUnit { get; set; }
        public string? Designation { get; set; }
        public string? Company { get; set; }

        public string? Category { get; set; }
        public string? Period { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? Days { get; set; }
        public DateOnly? AppliedOn { get; set; }
        public string? Status { get; set; }

        public string? Reason { get; set; }
        public string? Remarks { get; set; }
        public string? Address { get; set; }
        public string? Attachment { get; set; }
        public string? AttachmentUrl { get; set; }

        public List<EmployeeRecordDetailField> Fields { get; set; } = [];
        public List<EmployeeRecordEvent> Events { get; set; } = [];
    }

    public class EmployeeRecordDetailField
    {
        public string Label { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class EmployeeRecordEvent
    {
        public string? Category { get; set; }
        public string? EventStatus { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Remarks { get; set; }
        public string? SenderDesignation { get; set; }
        public string? ReceiverDesignation { get; set; }
    }
}
