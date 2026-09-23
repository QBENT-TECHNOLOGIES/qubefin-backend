using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record CreateEmployeeCommand(string Code, string? Salutation, string FirstName, string? MiddleName, string LastName, string? FatherName, string? MotherName,
    string? HusbandName, DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup, string? DisablityType, string? MaritalStatus,
    Guid CreatedBy
) : IRequest<Result<CreateEmployeeResponse?>>;
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
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date of Birth must be a valid date in the past.");
        RuleFor(x => x.Gender)
            .NotEmpty()
            .Must(value => value == "Male" || value == "Female" || value == "Other")
            .WithMessage("Gender must be either 'Male', 'Female', or 'Other'.");
        //RuleFor(x => x.BloodGroup).NotEmpty().WithMessage("Blood Group is Mandatory.");
        //RuleFor(x => x.Religion).NotEmpty().WithMessage("Religion is Mandatory.");
        RuleFor(x => x.Nationality).NotEmpty().WithMessage("Nationality is Mandatory.");
    }
}
#endregion
#region --- RESPONSE ---
public record CreateEmployeeResponse(Guid Id, string? Message);

#endregion

#region --- HANDLER ---
internal sealed class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmployeeCommand, Result<CreateEmployeeResponse?>>
{
    public async Task<Result<CreateEmployeeResponse?>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingEmployee = await employeeRepository.GetExsitingEmployeeByCode(null, request.Code);
            if (existingEmployee)
            {
                return new ValidationError("Employee already exist with same code.");
            }
            Guid employeeId = Guid.NewGuid();
            var employee = Employee.Create(
                employeeId,
                request.Code,
                new PersonalInfo(request.Code, request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName, request.HusbandName,
                    request.DateOfBirth, request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisablityType, request.MaritalStatus),
                new OfficialInfo(),
                new ContactInfo(),
                new AddressInfo(),
                new AddressInfo(),
                new PayrollInfo(),
                request.CreatedBy
                );

            await employeeRepository.AddAsync(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(new CreateEmployeeResponse(employeeId, $"Employee created successfully with Name : {request.FirstName} {request.LastName}"));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"An error occurred while creating the employee: {ex.Message}"));
        }
    }
}
#endregion
