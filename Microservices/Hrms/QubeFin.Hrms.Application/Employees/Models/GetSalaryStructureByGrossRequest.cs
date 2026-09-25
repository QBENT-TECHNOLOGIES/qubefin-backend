namespace QubeFin.Hrms.Application.Employees.Models;

public class GetSalaryStructureByGrossRequest
{
    public Guid EmployeeId { get; set; }
    public Guid SalaryGradeId { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal? FixedPFamount { get; set; }
}
