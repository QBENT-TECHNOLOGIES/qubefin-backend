using Microsoft.AspNetCore.Http;
using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class AttendanceSearchRequest : SearchParam
    {
        public Guid? CompanyId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? Status { get; set; }
    }
    public class AttendanceApprovalSearchRequest : SearchParam
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public Guid? SearchEmployeeId { get; set; }
    }
    public class AttendancePunchRequest
    {
        public Guid OrganizationUnitId { get; set; }
        public TimeOnly Time { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }

        public decimal? StartMileage { get; set; }
        public IFormFile? StartMileagePhoto { get; set; }

        public decimal? EndMileage { get; set; }
        public IFormFile? EndMileagePhoto { get; set; }
        public decimal? PersonalUseKm { get; set; }
    }
}
