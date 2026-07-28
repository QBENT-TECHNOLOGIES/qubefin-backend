using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class AttendanceSearchRequest : SearchParam
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? Status { get; set; }
    }
}
