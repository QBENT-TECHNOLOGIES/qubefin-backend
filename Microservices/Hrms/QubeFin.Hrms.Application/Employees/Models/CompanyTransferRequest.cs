using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Models
{
    public class CompanyTransferRequest
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid OrganisationUnitId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public Guid GradeId { get; set; }
        public decimal GrossSalary { get; set; }
        public DateOnly SeparationDate { get; set; }
        public DateOnly JoiningDate { get; set; }
    }
}
