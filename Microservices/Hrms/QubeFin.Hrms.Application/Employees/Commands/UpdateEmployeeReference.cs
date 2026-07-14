using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeReferenceCommand(
        Guid Id, string? Salutation, string FirstName, string? MiddleName,
        string LastName, string? Code, string? FatherName, string? MotherName, Guid OrganizationUnitId,
        Guid DepartmentId, string? EmployementType, DateOnly DateOfJoining, DateOnly? DateOfConfirmation,
        DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup,
        string? DisablityType, string? MaritalStatus, string MobileNo, string? PersonalEmail, string? EmergencyContactRelation1,
        string? EmergencyContactName1, string? EmergencyContactMobile1, string? EmergencyContactRelation2,
        string? EmergencyContactName2, string? EmergencyContactMobile2, string? PermanentHouseNo, string? PermanentRoadName,
        string? PermanentLandMark, Guid? PermanentAdministrativeUnitId, Guid? PermanentPoliceStationId, Guid? PermanentPostOfficeId,
        string? PermanentPinCode, string? PermanentOwnerShipOfHouse, int? PermanentDurationOfStayInMonths,
        string PresentHouseNo, string? PresentRoadName, string? PresentLandMark, Guid? PresentAdministrativeUnitId,
        Guid? PresentPoliceStationId, Guid? PresentPostOfficeId, string? PresentPinCode,
        string? PresentOwnerShipOfHouse, int? PresentDurationOfStayInMonths, Guid? BankId, long? BankAccountNo,
        string? BankHolderName, string? BankBranch, string? BankAccountType, string? OfficialEmail,
        bool? IsActive, bool? IsPayrollActive, Guid? CompanyId, DateOnly? SeparationDate, Guid? ReferedBy,
        string? HowYouKnow, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeReferenceResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeReferenceCommandValidator : AbstractValidator<UpdateEmployeeReferenceCommand>
    {
        //public UpdateEmployeeReferenceCommandValidator()
        //{
        //    RuleFor(x => x.FirstName)
        //        .Must(value => !string.IsNullOrWhiteSpace(value)
        //            && Regex.IsMatch(value, @"^[A-Za-z]+$")
        //            && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
        //        .WithMessage("Please enter a valid First Name name.")
        //        .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
        //        .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
        //    RuleFor(x => x.LastName)
        //        .NotEmpty()
        //        .Matches("^[A-Za-z]{3,30}$")
        //        .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
        //}
    }
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeReferenceResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeReferenceCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeReferenceCommand, Result<UpdateEmployeeReferenceResponse>>
    {
        public async Task<Result<UpdateEmployeeReferenceResponse>> Handle(UpdateEmployeeReferenceCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }


            //existingEmployee.UpdateEmployeeReferences(
            //    request.Salutation,
            //    request.FirstName,
            //    request.MiddleName,
            //    request.LastName,
            //    request.FatherName,
            //    request.MotherName,
            //    request.DateOfBirth,
            //    request.Gender,
            //    request.Religion,
            //    request.Caste,
            //    request.Nationality,
            //    request.BloodGroup,
            //    request.DisablityType,
            //    request.MaritalStatus,
            //    request.MobileNo,
            //    request.PersonalEmail,
            //    request.LastModifiedBy
            //    );
            //employeeRepository.UpdateEmployee(existingEmployee);

            //await unitOfWork.SaveChangesAsync();
            return Result.Ok(new UpdateEmployeeReferenceResponse(true));


        }
    }
    #endregion
}
