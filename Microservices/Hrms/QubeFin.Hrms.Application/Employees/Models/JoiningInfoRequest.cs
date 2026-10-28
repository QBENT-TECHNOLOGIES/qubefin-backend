namespace QubeFin.Hrms.Application.Employees.Models;

public class JoiningInfoRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly OfferLetterDate { get; set; }
    public decimal MonthlyCtc { get; set; }
}

public class JoiningInfoResponse
{
    public Guid? Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? ReferenceNo { get; set; }
    public DateOnly? OfferLetterDate { get; set; }
    public decimal? MonthlyCtc { get; set; }
    public string? ApprovalStatus { get; set; }
}
