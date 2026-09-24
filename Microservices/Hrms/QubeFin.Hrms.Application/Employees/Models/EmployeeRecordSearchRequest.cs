using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.Employees.Models
{
    public static class EmployeeRecordTypes
    {
        public const string Leave = "leave";
        public const string Regularization = "regularization";
        public const string Prayer = "prayer";
        public const string Attendance = "attendance";
        public const string Fitness = "fitness";

        public static readonly string[] All = [Leave, Regularization, Prayer, Attendance, Fitness];

        public static bool IsValid(string? recordType) =>
            !string.IsNullOrWhiteSpace(recordType) &&
            All.Contains(recordType.Trim().ToLowerInvariant());

        public static string Normalize(string? recordType) =>
            string.IsNullOrWhiteSpace(recordType) ? Leave : recordType.Trim().ToLowerInvariant();
    }

    public class EmployeeRecordSearchRequest : SearchParam
    {
        public string? RecordType { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public Guid? SearchEmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? Status { get; set; }

        /// <summary>
        /// Counts require querying every record type, so paging within a tab can skip them.
        /// </summary>
        public bool IncludeCounts { get; set; } = true;
    }

    public class EmployeeRecordItem
    {
        public Guid Id { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? OrganizationUnit { get; set; }

        public string? Category { get; set; }
        public string? Period { get; set; }
        public string? Quantity { get; set; }
        public string? Stage { get; set; }
        public string? Status { get; set; }

        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public DateOnly? AppliedOn { get; set; }
        public int? Days { get; set; }

        public string? WorkingHours { get; set; }
        public string? Reason { get; set; }
        public string? Attachment { get; set; }
    }

    public enum EmployeeRecordStatusFilter
    {
        All,
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public class EmployeeRecordCounts
    {
        public int Leave { get; set; }
        public int Regularization { get; set; }
        public int Prayer { get; set; }
        public int Attendance { get; set; }
        public int Fitness { get; set; }
    }

    public class EmployeeRecordStatusCounts
    {
        public int All { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Cancelled { get; set; }
    }

    public static class EmployeeRecordStatuses
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public const string Lapsed = "Lapsed";
    }
}
