using Microsoft.AspNetCore.Http;

namespace QubeFin.Hrms.Application.Employees.Models;

public class EmploymentDetailRequest
{
    public Guid Id { get; set; }
    public string EmployerName { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public decimal LastDrawnSalary { get; set; }
    public string? JobTitle { get; set; }
    public IFormFile? NocFile { get; set; }
    public string? NocFileName { get; set; }
    public string? NocFileNo { get; set; }
    public IFormFile? ExpCertFile { get; set; }
    public string? ExpCertFileName { get; set; }
    public string? ExpCertFileNo { get; set; }
    public Guid EmployeeId { get; set; }
    public int Sequence { get; set; }
}
public class EmploymentDetailResponse
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployerName { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public decimal LastDrawnSalary { get; set; }
    public string? JobTitle { get; set; }
    public string? NocFileName { get; set; }
    public string? NocFileUrl { get; set; }
    public string? ExpCertFileName { get; set; }
    public string? ExpCertFileUrl { get; set; }
    public int Sequence { get; set; }
}
