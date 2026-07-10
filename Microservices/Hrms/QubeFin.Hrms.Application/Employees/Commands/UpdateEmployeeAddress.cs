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
    public record UpdateEmployeeAddressCommand(
        Guid Id, string? EmergencyContactRelation1,
        string? EmergencyContactName1, string? EmergencyContactMobile1, string? EmergencyContactRelation2,
        string? EmergencyContactName2, string? EmergencyContactMobile2, string? PermanentHouseNo, string? PermanentRoadName,
        string? PermanentLandMark, Guid? PermanentAdministrativeUnitId, Guid? PermanentPoliceStationId, Guid? PermanentPostOfficeId,
        string? PermanentPinCode, string? PermanentOwnerShipOfHouse, int? PermanentDurationOfStayInMonths,
        string PresentHouseNo, string? PresentRoadName, string? PresentLandMark, Guid? PresentAdministrativeUnitId,
        Guid? PresentPoliceStationId, Guid? PresentPostOfficeId, string? PresentPinCode,
        string? PresentOwnerShipOfHouse, int? PresentDurationOfStayInMonths, Guid LastModifiedBy
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
            var existingEmployee = await employeeRepository.GetById(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }


            existingEmployee.UpdateEmployeeAddressDetails(
                request.PermanentHouseNo,
                request.PermanentRoadName,
                request.PermanentLandMark,
                request.PermanentAdministrativeUnitId,
                request.PermanentPoliceStationId,
                request.PermanentPostOfficeId,
                request.PermanentPinCode,
                request.PermanentOwnerShipOfHouse,
                request.PermanentDurationOfStayInMonths,
                request.PresentHouseNo,
                request.PresentRoadName,
                request.PresentLandMark,
                request.PresentAdministrativeUnitId,
                request.PresentPoliceStationId,
                request.PresentPostOfficeId,
                request.PresentPinCode,
                request.PresentOwnerShipOfHouse,
                request.PresentDurationOfStayInMonths,

                request.EmergencyContactRelation1,
                request.EmergencyContactName1,
                request.EmergencyContactMobile1,
                request.EmergencyContactRelation2,
                request.EmergencyContactName2,
                request.EmergencyContactMobile2,
                request.LastModifiedBy
                    );
                employeeRepository.UpdateEmployee(existingEmployee);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Ok(new UpdateEmployeeAddressResponse(true));


        }
    }
    #endregion
}
