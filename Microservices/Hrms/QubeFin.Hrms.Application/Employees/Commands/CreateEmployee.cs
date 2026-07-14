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
        string LastName, string Code, string? FatherName, string? MotherName,
        DateTime DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup,
        string? DisablityType, string? MaritalStatus, string MobileNo, string? PersonalEmail, Guid CreatedBy
    ) : IRequest<Result<CreateEmployeeResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.Id)
                .Equal(Guid.Empty)
                .WithMessage("Id must be empty guid.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
                .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.")
                .Matches(@"^[A-Za-z]+$").WithMessage("First Name must contain only letters.")
                .NotEqual("Select", StringComparer.OrdinalIgnoreCase).WithMessage("Please enter a valid First Name.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Matches(@"^[A-Za-z]{3,30}$")
                .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Matches(@"^\d{6}$")
                .WithMessage("Code must contain only numbers (0–9) and be exactly 6 digits long.");
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
            var employee = Employee.CreatePersonalDetails(
                Guid.NewGuid(),
                request.Salutation,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.Code,
                request.FatherName,
                request.MotherName,
                DateOnly.FromDateTime(request.DateOfBirth),
                request.Gender,
                request.Religion,
                request.Caste,
                request.Nationality,
                request.BloodGroup,
                request.DisablityType,
                request.MaritalStatus,
                request.MobileNo,
                request.PersonalEmail,
                request.CreatedBy
                );
            await employeeRepository.CreateEmployee(employee);

            await unitOfWork.SaveChangesAsync();
            return Result.Ok(new CreateEmployeeResponse(true));


        }
    }
    #endregion
}
