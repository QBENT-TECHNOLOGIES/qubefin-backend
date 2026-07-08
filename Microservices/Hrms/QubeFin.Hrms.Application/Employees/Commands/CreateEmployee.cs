using FluentResults;
using FluentValidation;
using MediatR;
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
    public record CreateEmployeeCommand(Employee employee) : IRequest<Result<CreateEmployeeResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.employee.FirstName)
                .Must(value => !string.IsNullOrWhiteSpace(value)
                    && Regex.IsMatch(value, @"^[A-Za-z]+$")
                    && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Please enter a valid First Name name.")
                .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
                .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
            RuleFor(x => x.employee.LastName)
                .NotEmpty()
                .Matches("^[A-Za-z]{3,30}$")
                .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            RuleFor(x => x.employee.Code)
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
            var existingEmployee = await context.TblEmployees.FirstOrDefaultAsync(m => m.Code == request.employee.Code);
            if(existingEmployee != null)
            {
                return new ValidationError("Employee already exist with same code.");
            }
            var employee = Employee.Create(
                Guid.NewGuid(),
                request.employee.Salutation,
                request.employee.FirstName,
                request.employee.MiddleName,
                request.employee.LastName,
                request.employee.Code,
                request.employee.FatherName,
                request.employee.MotherName,
                request.employee.OrganizationUnitId,
                request.employee.DepartmentId,
                request.employee.EmployementType,
                request.employee.DateOfJoining,
                request.employee.DateOfConfirmation,
                request.employee.DateOfBirth,
                request.employee.Gender,
                request.employee.Religion,
                request.employee.Caste,
                request.employee.Nationality,
                request.employee.BloodGroup,
                request.employee.DisablityType,
                request.employee.MaritalStatus,
                request.employee.MobileNo,
                request.employee.PersonalEmail,
                request.employee.EmergencyContactRelation1,
                request.employee.EmergencyContactName1,
                request.employee.EmergencyContactMobile1,
                request.employee.EmergencyContactRelation2,
                request.employee.EmergencyContactName2,
                request.employee.EmergencyContactMobile2,
                request.employee.PermanentHouseNo,
                request.employee.PermanentRoadName,
                request.employee.PermanentLandMark,
                request.employee.PermanentAdministrativeUnitId,
                request.employee.PermanentPoliceStationId,
                request.employee.PermanentPostOfficeId,
                request.employee.PermanentPinCode,
                request.employee.PermanentOwnerShipOfHouse,
                request.employee.PermanentDurationOfStayInMonths,
                request.employee.PresentHouseNo,
                request.employee.PresentRoadName,
                request.employee.PresentLandMark,
                request.employee.PresentAdministrativeUnitId,
                request.employee.PresentPoliceStationId,
                request.employee.PresentPostOfficeId,
                request.employee.PresentPinCode,
                request.employee.PresentOwnerShipOfHouse,
                request.employee.PresentDurationOfStayInMonths,
                request.employee.BankId,
                request.employee.BankAccountNo,
                request.employee.BankHolderName,
                request.employee.BankBranch,
                request.employee.BankAccountType,
                request.employee.OfficialEmail,
                request.employee.CompanyId,
                request.employee.SeparationDate,
                request.employee.ReferedBy,
                request.employee.HowYouKnow,
                request.employee.CreatedBy
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
