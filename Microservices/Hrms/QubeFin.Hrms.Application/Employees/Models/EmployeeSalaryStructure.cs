namespace QubeFin.Hrms.Application.Employees.Models;

public class EmployeeSalaryStructure
{
    public Guid SalaryComponentId { get; set; }
    public decimal Percentage { get; set; }
    public decimal MonthlyAmount { get; set; }
    public int NoOfDaysInMonth { get; set; }
    public int PayRollDayCount { get; set; }
    public string Category { get; set; }
    public string ComponentCode { get; set; }
    public decimal FixedAmount { get; set; }
}
