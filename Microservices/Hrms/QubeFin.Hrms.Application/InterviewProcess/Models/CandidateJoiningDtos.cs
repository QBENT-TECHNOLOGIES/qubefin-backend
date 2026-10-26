using Microsoft.AspNetCore.Http;
using QubeFin.Hrms.Application.Employees.Models;

namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public static class CandidateJoiningDocumentCategory
{
    public const string Photo = "PHOTO";
    public const string Signature = "SIGNATURE";
}

public class CandidateJoiningPersonalRequest
{
    public string Code { get; set; } = null!;
    public string? Salutation { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? HusbandName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Religion { get; set; } = null!;
    public string? Caste { get; set; }
    public string Nationality { get; set; } = null!;
    public string BloodGroup { get; set; } = null!;
    public string? DisablityType { get; set; }
    public string? MaritalStatus { get; set; }
    public IFormFile? Photo { get; set; }
    public IFormFile? Signature { get; set; }
}
public record CandidateJoiningInfoResponse(
    Guid CandidateId,
    Guid? EmployeeId,
    string? EmployeeCode,
    string MobileNo,
    bool IsMobileValidated,
    string? Email,
    string? AadharNumber,
    bool IsAadharValidated,
    string? VoterNumber,
    bool IsVoterValidated,
    string? Pan,
    bool IsPanValidated,
    string? Uan,
    bool IsUanVerified,
    AddressInfoResponse? Address,
    Guid? CompanyId,
    Guid? OrganizationUnitTypeId,
    Guid? OrganizationUnitId,
    Guid? DepartmentId,
    DateOnly? DateOfJoining,
    Guid? DesignationId);
public record CandidateJoiningPersonalResponse(
    Guid? EmployeeId,
    string? Code,
    string? Salutation,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? FatherName,
    string? MotherName,
    string? HusbandName,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Religion,
    string? Caste,
    string? Nationality,
    string? BloodGroup,
    string? DisablityType,
    string? MaritalStatus,
    string? PhotoFileName,
    string? PhotoFileUrl,
    string? SignatureFileName,
    string? SignatureFileUrl);
