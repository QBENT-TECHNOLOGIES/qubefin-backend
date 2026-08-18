using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Attendances.Models
{
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
        public string? IsRegulerized { get; set; }
    }
}
