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
    public record UpdateEmployeePersonalDetailCommand(
        Guid Id, string? Salutation, string FirstName, string? MiddleName,
        string LastName, string? Code, string? FatherName, string? MotherName, 
        DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup,
        string? DisablityType, string? MaritalStatus, string MobileNo, string? PersonalEmail, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeePersonalDetailCommandValidator : AbstractValidator<UpdateEmployeePersonalDetailCommand>
    {
        public UpdateEmployeePersonalDetailCommandValidator()
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
            
        }
    }
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeePersonalDetailCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeePersonalDetailCommand, Result<UpdateEmployeeResponse>>
    {
        public async Task<Result<UpdateEmployeeResponse>> Handle(UpdateEmployeePersonalDetailCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }


            //existingEmployee.UpdateEmployeePersonalDetails(
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

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeResponse(true));


        }
    }
    #endregion
}
