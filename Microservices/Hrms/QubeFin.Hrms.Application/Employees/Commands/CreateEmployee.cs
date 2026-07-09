using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record CreateEmployeeCommand(Guid Id, string? Salutation, string FirstName, string? MiddleName,
        string LastName, string? Code, string? FatherName, string? MotherName, Guid OrganizationUnitId,
        Guid DepartmentId, string? EmployementType, DateOnly DateOfJoining, DateOnly? DateOfConfirmation,
        DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup,
        string? DisablityType, string? MaritalStatus, string MobileNo, string? PersonalEmail,  string? EmergencyContactRelation1,
        string? EmergencyContactName1, string? EmergencyContactMobile1, string? EmergencyContactRelation2,
        string? EmergencyContactName2, string? EmergencyContactMobile2, string? PermanentHouseNo, string? PermanentRoadName,
        string? PermanentLandMark, Guid? PermanentAdministrativeUnitId, Guid? PermanentPoliceStationId, Guid? PermanentPostOfficeId,
        string? PermanentPinCode, string? PermanentOwnerShipOfHouse, int? PermanentDurationOfStayInMonths,
        string PresentHouseNo, string? PresentRoadName, string? PresentLandMark, Guid? PresentAdministrativeUnitId,
        Guid? PresentPoliceStationId, Guid? PresentPostOfficeId, string? PresentPinCode,
        string? PresentOwnerShipOfHouse, int? PresentDurationOfStayInMonths, Guid? BankId, long? BankAccountNo,
        string? BankHolderName, string? BankBranch, string? BankAccountType, string? OfficialEmail,
        bool? IsActive, bool? IsPayrollActive, Guid? CompanyId, DateOnly? SeparationDate, Guid? ReferedBy,
        string? HowYouKnow, Guid CreatedBy
    ) : IRequest<Result<CreateEmployeeResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .Must(value => !string.IsNullOrWhiteSpace(value)
                    && Regex.IsMatch(value, @"^[A-Za-z]+$")
                    && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Please enter a valid First Name name.")
                .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
                .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .Matches("^[A-Za-z]{3,30}$")
                .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches("^[0-9]{6}$")
                .WithMessage("Code must contain only numbers (0–9) and 6 digits long.");
        }
    }
    #endregion

    #region --- RESPONSE ---
    public record CreateEmployeeResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<CreateEmployeeCommand, Result<CreateEmployeeResponse>>
    {
        public async Task<Result<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await context.TblEmployees.FirstOrDefaultAsync(m => m.Code == request.Code);
            if(existingEmployee != null)
            {
                return new ValidationError("Employee already exist with same code.");
            }
            var employee = Employee.Create(
                Guid.NewGuid(),
                request.Salutation,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.Code,
                request.FatherName,
                request.MotherName,
                request.OrganizationUnitId,
                request.DepartmentId,
                request.EmployementType,
                request.DateOfJoining,
                request.DateOfConfirmation,
                request.DateOfBirth,
                request.Gender,
                request.Religion,
                request.Caste,
                request.Nationality,
                request.BloodGroup,
                request.DisablityType,
                request.MaritalStatus,
                request.MobileNo,
                request.PersonalEmail,
                request.EmergencyContactRelation1,
                request.EmergencyContactName1,
                request.EmergencyContactMobile1,
                request.EmergencyContactRelation2,
                request.EmergencyContactName2,
                request.EmergencyContactMobile2,
                request.PermanentHouseNo,
                request.PermanentRoadName,
                request.PermanentLandMark,
                request.PermanentAdministrativeUnitId,
                request.PermanentPoliceStationId,
                request.PermanentPostOfficeId,
                request.PermanentPinCode,
                request.PermanentOwnerShipOfHouse,
                request.PermanentDurationOfStayInMonths,
                request.PresentHouseNo,
                request.PresentRoadName,
                request.PresentLandMark,
                request.PresentAdministrativeUnitId,
                request.PresentPoliceStationId,
                request.PresentPostOfficeId,
                request.PresentPinCode,
                request.PresentOwnerShipOfHouse,
                request.PresentDurationOfStayInMonths,
                request.BankId,
                request.BankAccountNo,
                request.BankHolderName,
                request.BankBranch,
                request.BankAccountType,
                request.OfficialEmail,
                request.CompanyId,
                request.SeparationDate,
                request.ReferedBy,
                request.HowYouKnow,
                request.CreatedBy
                );
            //var employeeCreate = Employee.AddEmployee(request.employee);
            //await employeeRepository.CreateEmployee(employeeCreate);
            await employeeRepository.CreateEmployee(employee);

            await unitOfWork.SaveChangesAsync();
            return Result.Ok(new CreateEmployeeResponse(true));


        }
    }
    #endregion
}
