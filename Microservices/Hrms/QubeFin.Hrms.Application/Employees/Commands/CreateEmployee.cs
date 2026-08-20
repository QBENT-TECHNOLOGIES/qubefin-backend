using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record CreateEmployeeCommand(string Code, string? Salutation, string FirstName, string? MiddleName, string LastName, string? FatherName, string? MotherName, 
    DateTime DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup, string? DisabilityType, string? MaritalStatus,
    Guid CreatedBy
) : IRequest<Result<string>>;
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
            .Matches("^[A-Za-z0-9]+$")
            .WithMessage("Code must contain only numbers (0–9) and 6 digits long.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmployeeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingEmployee = await employeeRepository.GetExsitingEmployeeByCode(null, request.Code);
        if (existingEmployee)
        {
            return new ValidationError("Employee already exist with same code.");
        }

        var employee = Employee.Create(
            Guid.NewGuid(),
            request.Code,
            new PersonalInfo(request.Code, request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName,
                DateOnly.FromDateTime( request.DateOfBirth), request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisabilityType, request.MaritalStatus),
            new OfficialInfo(),
            new ContactInfo(),
            new AddressInfo(),
            new AddressInfo(),
            new PayrollInfo(),
            request.CreatedBy
            );

        await employeeRepository.AddAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"Employee created successfully with Name : {request.FirstName} {request.LastName}");
    }
}
#endregion
