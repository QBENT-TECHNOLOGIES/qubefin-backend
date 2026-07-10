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
    public record UpdateEmployeeOfficialCommand(Guid Id,Guid OrganizationUnitId,
        Guid DepartmentId, string? EmployementType, DateOnly DateOfJoining, DateOnly? DateOfConfirmation,
        Guid BankId, long BankAccountNo,
        string BankHolderName, string BankBranch, string BankAccountType, DateOnly? SeparationDate, Guid? ReferedBy,
        string? HowYouKnow, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeOfficialResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeOfficialCommandValidator : AbstractValidator<UpdateEmployeeOfficialCommand>
    {
        public UpdateEmployeeOfficialCommandValidator()
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
    public record UpdateEmployeeOfficialResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeOfficialCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeOfficialCommand, Result<UpdateEmployeeOfficialResponse>>
    {
        public async Task<Result<UpdateEmployeeOfficialResponse>> Handle(UpdateEmployeeOfficialCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetById(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }


            existingEmployee.UpdateEmployeeOfficialDetails(

                request.OrganizationUnitId,
                request.DepartmentId,
                request.EmployementType,
                request.DateOfJoining,
                request.DateOfConfirmation,
                request.SeparationDate,
                request.ReferedBy,
                request.HowYouKnow,
                request.BankId,
                request.BankAccountNo,
                request.BankHolderName,
                request.BankBranch,
                request.BankAccountType,
                request.LastModifiedBy


                
                );
            employeeRepository.UpdateEmployee(existingEmployee);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeOfficialResponse(true));


        }
    }
    #endregion
}
