namespace QubeFin.Hrms.Application.Employees.Models;

public class JoiningInfoRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly OfferLetterDate { get; set; }
    public decimal MonthlyCtc { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
}

public static class JoiningInfoApprovalStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
}

public class JoiningInfoActionRequest
{
    public Guid JoiningInfoId { get; set; }
    public bool IsApproved { get; set; }
}

public class JoiningInfoApprovalListItem
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public string? Designation { get; set; }
    public string? OrganizationUnit { get; set; }
    public string? Company { get; set; }
    public string? Department { get; set; }
    public string? SalaryGrade { get; set; }
    public decimal MonthlyCtc { get; set; }
    public DateOnly OfferLetterDate { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string? Qualification { get; set; }
    public string ApprovalStatus { get; set; } = JoiningInfoApprovalStatus.Pending;
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
