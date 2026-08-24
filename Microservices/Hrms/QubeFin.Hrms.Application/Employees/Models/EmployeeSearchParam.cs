using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Models
{
    public class EmployeeSearchParam : SearchParam
    {
        public Guid? SearchOrganizationUnitId { get; set; }
        public Guid? CompanyId { get; set; }
        public DateOnly? SrchJoiningDate { get; set; }
        public string? SearchType { get; set; }
    }
    public class EmployeeSearchResult
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CompanyName { get; set; }
        public string? OrganizationUnitName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Gender { get; set; }
        public DateOnly? JoiningDate { get; set; }
        public DateOnly? SeperationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
