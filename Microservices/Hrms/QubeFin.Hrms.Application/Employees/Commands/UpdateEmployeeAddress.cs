using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeAddressCommand(
        Guid Id, AddressModel? PresentAddress, AddressModel? PermanentAddress,
        Guid UserId
        ) : IRequest<Result<string>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeAddressCommandValidator : AbstractValidator<UpdateEmployeeAddressCommand>
    {
        public UpdateEmployeeAddressCommandValidator()
        {
            RuleFor(x => x.PresentAddress!.PostOfficeName).MaximumLength(50).WithMessage("Present Post Office cannot exceed 50 characters.")
                .When(x => x.PresentAddress != null);
            RuleFor(x => x.PermanentAddress!.PostOfficeName).MaximumLength(50).WithMessage("Permanent Post Office cannot exceed 50 characters.")
                .When(x => x.PermanentAddress != null);
        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeAddressCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeAddressCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UpdateEmployeeAddressCommand request, CancellationToken cancellationToken)
        {
            var employee = await employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
            {
                return new ValidationError("Employee does not exist with the given id.");
            }

            // Post office typed in manually (not found for the pin code): reuse or add it
            var presentPostOfficeId = await ResolvePostOfficeAsync(request.PresentAddress, request.UserId, cancellationToken);
            var permanentPostOfficeId = await ResolvePostOfficeAsync(request.PermanentAddress, request.UserId, cancellationToken);

            employee.UpdateAddressInfo(
                new AddressInfo(
                    request.PresentAddress?.HouseNo,
                    request.PresentAddress?.RoadName,
                    request.PresentAddress?.LandMark,
                    request.PresentAddress?.AdministrativeUnitId,
                    request.PresentAddress?.PoliceStationId,
                    presentPostOfficeId,
                    request.PresentAddress?.PinCode,
                    request.PresentAddress?.OwnerShipOfHouse,
                    request.PresentAddress?.DurationOfStayInMonths),


                new AddressInfo(
                    request.PermanentAddress?.HouseNo,
                    request.PermanentAddress?.RoadName,
                    request.PermanentAddress?.LandMark,
                    request.PermanentAddress?.AdministrativeUnitId,
                    request.PermanentAddress?.PoliceStationId,
                    permanentPostOfficeId,
                    request.PermanentAddress?.PinCode,
                    request.PermanentAddress?.OwnerShipOfHouse,
                    request.PermanentAddress?.DurationOfStayInMonths),
                request.UserId
                );

            await employeeRepository.UpdateAsync(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Employee address information updated successfully for Name : {employee.PersonalInfo.FirstName} {employee.PersonalInfo.LastName}");
        }

        // Returns the selected post office, or finds/creates the manually entered one for the pin code
        private async Task<Guid?> ResolvePostOfficeAsync(AddressModel? address, Guid userId, CancellationToken cancellationToken)
        {
            if (address is null || address.PostOfficeId is not null)
            {
                return address?.PostOfficeId;
            }

            var name = address.PostOfficeName?.Trim();
            var pinCode = address.PinCode?.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pinCode))
            {
                return null;
            }

            // Check pending (present address added just now) and saved post offices
            var existing = context.TblPostOffices.Local
                    .FirstOrDefault(p => p.Pincode == pinCode && string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                ?? await context.TblPostOffices
                    .FirstOrDefaultAsync(p => p.Pincode == pinCode && p.Name == name, cancellationToken);
            if (existing is not null)
            {
                return existing.Id;
            }

            var postOffice = new TblPostOffice
            {
                Id = Guid.NewGuid(),
                Name = name,
                Pincode = pinCode,
                CreatedOn = DateTime.Now,
                CreatedBy = userId
            };
            await context.TblPostOffices.AddAsync(postOffice, cancellationToken);
            return postOffice.Id;
        }
    }
    #endregion
}
