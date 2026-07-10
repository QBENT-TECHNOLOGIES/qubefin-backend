using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetAllEmployeeQuery : IRequest<Result<List<GetAllEmployeeResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetAllEmployeeResponse(
    Guid Id,
    string? Salutation,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Code,
    string? FatherName,
    string? MotherName,
    Guid? OrganizationUnitId,
    Guid? DepartmentId,
    string? EmployementType,
    DateOnly? DateOfJoining,
    DateOnly? DateOfConfirmation,
    DateOnly DateOfBirth,
    string Gender,
    string Religion,
    string? Caste,
    string Nationality,
    string BloodGroup,
    string? DisablityType,
    string? MaritalStatus,
    string MobileNo,
    string? PersonalEmail,
    string? EmergencyContactRelation1,
    string? EmergencyContactName1,
    string? EmergencyContactMobile1,
    string? EmergencyContactRelation2,
    string? EmergencyContactName2,
    string? EmergencyContactMobile2,
    string? PermanentHouseNo,
    string? PermanentRoadName,
    string? PermanentLandMark,
    Guid? PermanentAdministrativeUnitId,
    Guid? PermanentPoliceStationId,
    Guid? PermanentPostOfficeId,
    string? PermanentPinCode,
    string? PermanentOwnerShipOfHouse,
    int? PermanentDurationOfStayInMonths,
    string? PresentHouseNo,
    string? PresentRoadName,
    string? PresentLandMark,
    Guid? PresentAdministrativeUnitId,
    Guid? PresentPoliceStationId,
    Guid? PresentPostOfficeId,
    string? PresentPinCode,
    string? PresentOwnerShipOfHouse,
    int? PresentDurationOfStayInMonths,
    Guid? BankId,
    long? BankAccountNo,
    string? BankHolderName,
    string? BankBranch,
    string? BankAccountType,
    string? OfficialEmail,
    bool? IsActive,
    bool? IsPayrollActive,
    Guid? CompanyId,
    DateOnly? SeparationDate,
    Guid? ReferedBy,
    string? HowYouKnow
);
#endregion

#region --- HANDLER ---
internal sealed class GetAllEmployeeQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAllEmployeeQuery, Result<List<GetAllEmployeeResponse>>>
{
    public async Task<Result<List<GetAllEmployeeResponse>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await context
            .TblEmployees
            .Select(m => new GetAllEmployeeResponse( 
                m.Id,
                m.Salutation,
                m.FirstName,
                m.MiddleName,
                m.LastName,
                m.Code,
                m.FatherName,
                m.MotherName,
                m.OrganizationUnitId,
                m.DepartmentId,
                m.EmployementType,
                m.DateOfJoining,
                m.DateOfConfirmation,
                m.DateOfBirth,
                m.Gender,
                m.Religion,
                m.Caste,
                m.Nationality,
                m.BloodGroup,
                m.DisablityType,
                m.MaritalStatus,
                m.MobileNo,
                m.PersonalEmail,
                m.EmergencyContactRelation1,
                m.EmergencyContactName1,
                m.EmergencyContactMobile1,
                m.EmergencyContactRelation2,
                m.EmergencyContactName2,
                m.EmergencyContactMobile2,
                m.PermanentHouseNo,
                m.PermanentRoadName,
                m.PermanentLandMark,
                m.PermanentAdministrativeUnitId,
                m.PermanentPoliceStationId,
                m.PermanentPostOfficeId,
                m.PermanentPinCode,
                m.PermanentOwnerShipOfHouse,
                m.PermanentDurationOfStayInMonths,
                m.PresentHouseNo,
                m.PresentRoadName,
                m.PresentLandMark,
                m.PresentAdministrativeUnitId,
                m.PresentPoliceStationId,
                m.PresentPostOfficeId,
                m.PresentPinCode,
                m.PresentOwnerShipOfHouse,
                m.PresentDurationOfStayInMonths,
                m.BankId,
                m.BankAccountNo,
                m.BankHolderName,
                m.BankBranch,
                m.BankAccountType,
                m.OfficialEmail,
                m.IsActive,
                m.IsPayrollActive,
                m.CompanyId,
                m.SeparationDate,
                m.ReferedBy,
                m.HowYouKnow
            ))
            .ToListAsync(cancellationToken);

        
        return Result.Ok(employee);
    }
}
#endregion