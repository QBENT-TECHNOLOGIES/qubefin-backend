using QubeFin.Persistence.Models;

namespace QubeFin.Report.Application.Reports.Models
{
    public class AttendanceSearchRequest : SearchParam
    {
        public Guid CompanyId { get; set;  }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? Status { get; set; }
    }

    public class AttendanceSearchResult
    {
        public Guid Id { get; set; }
        public string? OrganizationUnit { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public string? ActualInTime { get; set; }
        public string? ActualOutTime { get; set; }
        public string? WorkingHours { get; set; }
        public string? Status { get; set; }
        public string? IsRegularized { get; set; }
    }
}
