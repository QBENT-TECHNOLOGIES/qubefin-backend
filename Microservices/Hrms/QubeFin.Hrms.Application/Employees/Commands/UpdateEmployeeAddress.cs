using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeAddressCommand(
        Guid Id, AddressModel? PresentAddress, AddressModel? PermanentAddress,
        Guid UserId
        ) : IRequest<Result<UpdateEmployeeAddressResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeAddressCommandValidator : AbstractValidator<UpdateEmployeeAddressCommand>
    {
        public UpdateEmployeeAddressCommandValidator()
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
    public record UpdateEmployeeAddressResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeAddressCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeAddressCommand, Result<UpdateEmployeeAddressResponse>>
    {
        public async Task<Result<UpdateEmployeeAddressResponse>> Handle(UpdateEmployeeAddressCommand request, CancellationToken cancellationToken)
        {
            var employee = await employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
            {
                return new ValidationError("Employee does not exist with the given id.");
            }

            employee.UpdateAddressInfo(
                new AddressInfo(
                    request.PresentAddress?.HouseNo,
                    request.PresentAddress?.RoadName,
                    request.PresentAddress?.LandMark,
                    request.PresentAddress?.AdministrativeUnitId,
                    request.PresentAddress?.PoliceStationId,
                    request.PresentAddress?.PostOfficeId,
                    request.PresentAddress?.PinCode,
                    request.PresentAddress?.OwnerShipOfHouse,
                    request.PresentAddress?.DurationOfStayInMonths),


                new AddressInfo(
                    request.PermanentAddress?.HouseNo,
                    request.PermanentAddress?.RoadName,
                    request.PermanentAddress?.LandMark,
                    request.PermanentAddress?.AdministrativeUnitId,
                    request.PermanentAddress?.PoliceStationId,
                    request.PermanentAddress?.PostOfficeId,
                    request.PermanentAddress?.PinCode,
                    request.PermanentAddress?.OwnerShipOfHouse,
                    request.PermanentAddress?.DurationOfStayInMonths),
                request.UserId
                );

            await employeeRepository.UpdateAsync(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeAddressResponse(true));


        }
    }
    #endregion
}
