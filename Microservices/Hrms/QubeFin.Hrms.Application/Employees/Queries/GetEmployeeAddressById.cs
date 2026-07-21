using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeAddressByIdQuery(Guid Id) : IRequest<Result<GetAddressResponse>>;
#endregion
#region --- RESPONSE ---
public record GetAddressResponse(
    Guid Id,
    string Code,
    AddressInfo PresentAddressInfo,
    AddressInfo PermanentAddressInfo
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeAddressByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeAddressByIdQuery, Result<GetAddressResponse>>
{
    public async Task<Result<GetAddressResponse>> Handle(GetEmployeeAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(new GetAddressResponse(
            Id: employee.Id,
            Code: employee.Code,
            PresentAddressInfo: new AddressInfo(
                employee.PresentHouseNo,
                employee.PresentRoadName,
                employee.PresentLandMark,
                employee.PresentAdministrativeUnitId,
                employee.PresentPoliceStationId,
                employee.PresentPostOfficeId,
                employee.PresentPinCode,
                employee.PresentOwnerShipOfHouse,
                employee.PresentDurationOfStayInMonths
            ),
            PermanentAddressInfo: new AddressInfo(
                    employee.PermanentHouseNo,
                    employee.PermanentRoadName,
                    employee.PermanentLandMark,
                    employee.PermanentAdministrativeUnitId,
                    employee.PermanentPoliceStationId,
                    employee.PermanentPostOfficeId,
                    employee.PermanentPinCode,
                    employee.PermanentOwnerShipOfHouse,
                    employee.PermanentDurationOfStayInMonths
                )
        ));
    }
}
#endregion