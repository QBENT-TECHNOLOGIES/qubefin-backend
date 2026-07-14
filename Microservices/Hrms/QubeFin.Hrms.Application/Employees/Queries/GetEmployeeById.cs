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
public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<GetEmployeeByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetEmployeeByIdResponse(
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
    DateOnly? JoiningDate,
    DateOnly? ConfirmationDate,
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
    bool IsActive,
    bool IsPayrollActive,
    Guid? CompanyId,
    DateOnly? SeparationDate,
    Guid? ReferedBy,
    string? HowYouKnow
);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeByIdQuery, Result<GetEmployeeByIdResponse>>
{
    public async Task<Result<GetEmployeeByIdResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context
            .TblEmployees
            .Where(m => m.Id == request.Id)
            .Select(m => new GetEmployeeByIdResponse( 
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
                m.JoiningDate,
                m.ConfirmationDate,
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
            .FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(employee);
    }
}
#endregion