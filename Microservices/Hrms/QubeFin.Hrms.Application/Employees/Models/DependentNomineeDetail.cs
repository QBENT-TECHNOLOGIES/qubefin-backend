namespace QubeFin.Hrms.Application.Employees.Models;

public class DependentNomineeDetailRequest
{
    public Guid Id { get; set; }
    public string NomineeName { get; set; } = null!;
    public string RelationWithInsuredPerson { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? UhidAbhaNumber { get; set; }
    public string? AbhaAddress { get; set; }
    public string? Uan { get; set; }
    public string? AadharNumber { get; set; }
    public string? VoterIdnumber { get; set; }
    public bool IsResidingWithIp { get; set; }
    public Guid? StateId { get; set; }
    public Guid? DistrictId { get; set; }
    public decimal? Percentage { get; set; }
}
public class DependentNomineeDetailResponse
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string NomineeName { get; set; } = null!;
    public string RelationWithInsuredPerson { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? UhidAbhaNumber { get; set; }
    public string? AbhaAddress { get; set; }
    public string? Uan { get; set; }
    public string? AadharNumber { get; set; }
    public string? VoterIdnumber { get; set; }
    public bool IsResidingWithIp { get; set; }
    public Guid? StateId { get; set; }
    public Guid? DistrictId { get; set; }
    public decimal? Percentage { get; set; }
}
