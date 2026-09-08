using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Models
{
    public class EmployeeTransferHistoryResponse
    {
        public Guid Id { get; set; }
        public string OrganisationUnit { get; set; } = string.Empty;
        public string OrganisationUnitType { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string SalaryGrade { get; set; } = string.Empty;
        public decimal? GrossSalary { get; set; } = 0;
        public DateOnly FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }

    public class EmployeeCurrentOfficialInfoResponse
    {
        public Guid? OrganisationUnitTypeId { get; set; }
        public Guid? OrganisationUnitId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? SalaryGradeId { get; set; }
        public decimal? GrossSalary { get; set; } = 0;
    }

    public class EmployeeCurrentOfficialInfoRequest
    {
        public Guid EmployeeId { get; set; }
        public Guid OrganisationUnitId { get; set; }
        public Guid DesignationId { get; set; }
        public Guid SalaryGradeId { get; set; }
        public decimal GrossSalary { get; set; } = 0;
    }
}
