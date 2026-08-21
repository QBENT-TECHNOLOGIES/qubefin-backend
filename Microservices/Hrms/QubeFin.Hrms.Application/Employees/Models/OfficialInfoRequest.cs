using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Models;

public class OfficialInfoRequest
{
    public Guid? CompanyId { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? EmployementType { get; set; }
    public DateOnly? DateOfJoining { get; set; }
    public DateOnly? DateOfConfirmation { get; set; }
    public DateOnly? SeparationDate { get; set; }
    public Guid? ReferedBy { get; set; }
    public string? HowYouKnow { get; set; }
    public string? OfficialEmail { get; set; }
    public decimal? GrossSalary { get; set; }
    public bool IsActive { get; set; }
}
