namespace QubeFin.Hrms.Application.Employees.Models;

public class JoiningInfoRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly OfferLetterDate { get; set; }
    public decimal MonthlyCtc { get; set; }
}
