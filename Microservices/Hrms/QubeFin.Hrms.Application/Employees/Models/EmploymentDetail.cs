namespace QubeFin.Hrms.Application.Employees.Models;

public class EmploymentDetailRequest
{
    public Guid Id { get; set; }
    public string EmployerName { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal LastDrawnSalary { get; set; }
    public string? JobTitle { get; set; }
    public string? NocFileName { get; set; }
    public string? NocFileNo { get; set; }
    public string? ExpCertFileName { get; set; }
    public string? ExpCertFileNo { get; set; }
    public Guid EmployeeId { get; set; }
    public int Sequence { get; set; }
}
