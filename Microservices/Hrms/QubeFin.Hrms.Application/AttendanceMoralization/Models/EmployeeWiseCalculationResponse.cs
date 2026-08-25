using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Models
{
    public class EmployeeWiseCalculationResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public string? OrganizationUnitName { get; set; }
        public string? CompanyName { get; set; }
        public int HoliDays { get; set; }
        public int WorkingDays { get; set; }
        public int LeaveDays { get; set; }
        public int AttendanceDays { get; set; }
        public int AbsentDays { get; set; }
        public int AttendanceIrregularDays { get; set; }
        public int IrregularLopDays { get; set; }
        public bool IsLocked { get; set; }
        public string? Remarks { get; set; }
    }
    public class EmployeeLosDetails
    {
        public Guid Id { get; set; }
        public Guid EmployeeLopId { get; set; }
        public DateOnly AbsentDate { get; set; }
        public string? PayrollStatus { get; set; }
    }
}
