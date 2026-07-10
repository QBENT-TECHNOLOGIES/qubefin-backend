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
    public record UpdateEmployeeQualificationCommand(
            string AcademicStream,
            string? Specialization,
            int YearOfPassing,
            string? UniversityOrBoard,
            string? SchoolOrCollege,
            string? GradeOrMarks,
            string? DocFileName,
            string? DocFileNo,
            Guid EmployeeId,
            int Sequence, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeQualificationResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeQualificationCommandValidator : AbstractValidator<UpdateEmployeeQualificationCommand>
    {
        public UpdateEmployeeQualificationCommandValidator()
        {
            //RuleFor(x => x.FirstName)
            //    .Must(value => !string.IsNullOrWhiteSpace(value)
            //        && Regex.IsMatch(value, @"^[A-Za-z]+$")
            //        && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            //    .WithMessage("Please enter a valid First Name name.")
            //    .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            //    .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
            //RuleFor(x => x.LastName)
            //    .NotEmpty()
            //    .Matches("^[A-Za-z]{3,30}$")
            //    .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
        }
    }
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeQualificationResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeQualificationCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeQualificationCommand, Result<UpdateEmployeeQualificationResponse>>
    {
        public async Task<Result<UpdateEmployeeQualificationResponse>> Handle(UpdateEmployeeQualificationCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetById(request.EmployeeId);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }


            //existingEmployee.UpdateEmployeeQualifications(
            //    Guid.NewGuid(),
            //    request.AcademicStream,
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
            return Result.Ok(new UpdateEmployeeQualificationResponse(true));


        }
    }
    #endregion
}
