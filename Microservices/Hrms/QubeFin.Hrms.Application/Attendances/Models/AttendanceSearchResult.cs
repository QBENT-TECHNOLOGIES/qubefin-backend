using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class AttendanceSearchResult
    {
        public Guid Id { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public TimeOnly? ActualInTime { get; set; }
        public TimeOnly? ActualOutTime { get; set; }
        public string? WorkingHours { get; set; }
        public string? Status { get; set; }
    }
}
