using QubeFin.Persistence.Models;

namespace QubeFin.Report.Application.Reports.Models
{
    public class LopFinalizationReportSearchRequest : SearchParam
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? SearchOrganizationUnitId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? Status { get; set; }
    }
    public class LopFinalizationReportResponse
    {
        public string? CompanyName { get; set; }
        public string? OrganizationUnitName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public int HoliDays { get; set; }
        public int WorkingDays { get; set; }
        public int LeaveDays { get; set; }
        public int AttendanceDays { get; set; }
        public int AbsentDays { get; set; }
        public int AttendanceIrregularDays { get; set; }
        public int IrregularLopDays { get; set; }
        public string? IsLocked { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}
