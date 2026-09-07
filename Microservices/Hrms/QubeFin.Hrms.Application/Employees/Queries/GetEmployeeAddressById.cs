using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeAddressByIdQuery(Guid Id) : IRequest<Result<GetAddressResponse>>;
#endregion
#region --- RESPONSE ---
public record GetAddressResponse(
    Guid Id,
    string Code,
    bool SameAsPresentAddress,
    AddressInfoResponse PresentAddressInfo,
    AddressInfoResponse PermanentAddressInfo
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeAddressByIdQueryHandler(QubeFinDataContext context, IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeAddressByIdQuery, Result<GetAddressResponse>>
{
    public async Task<Result<GetAddressResponse>> Handle(GetEmployeeAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var presentAddressUnit = await AddressUnit(employee.PresentAdministrativeUnitId);

        var permanentAddressUnit = await AddressUnit(employee.PermanentAdministrativeUnitId);

        return Result.Ok(new GetAddressResponse(
            Id: employee.Id,
            Code: employee.Code,
            SameAsPresentAddress: employee.PresentAdministrativeUnitId == employee.PermanentAdministrativeUnitId,
            PresentAddressInfo: new AddressInfoResponse
            {
                HouseNo = employee.PresentHouseNo,
                RoadName = employee.PresentRoadName,
                LandMark = employee.PresentLandMark,
                AdministrativeUnitId = employee.PresentAdministrativeUnitId,
                PoliceStationId = employee.PresentPoliceStationId,
                PostOfficeId = employee.PresentPostOfficeId,
                PinCode = employee.PresentPinCode,
                OwnerShipOfHouse = employee.PresentOwnerShipOfHouse,
                DurationOfStayInMonths = employee.PresentDurationOfStayInMonths,
                AddressUnit = presentAddressUnit
            },
            PermanentAddressInfo: new AddressInfoResponse
            {
                HouseNo = employee.PermanentHouseNo,
                RoadName = employee.PermanentRoadName,
                LandMark = employee.PermanentLandMark,
                AdministrativeUnitId = employee.PermanentAdministrativeUnitId,
                PoliceStationId = employee.PermanentPoliceStationId,
                PostOfficeId = employee.PermanentPostOfficeId,
                PinCode = employee.PermanentPinCode,
                OwnerShipOfHouse = employee.PermanentOwnerShipOfHouse,
                DurationOfStayInMonths = employee.PermanentDurationOfStayInMonths,
                AddressUnit = permanentAddressUnit
            }
        ));
    }

    private async Task<AddressUnit?> AddressUnit(Guid? id)
    {
        if (id != null)
        {
            return await employeeRepository.GetAdressUnit(id.Value);
        }

        return new AddressUnit();
    }
}
#endregion