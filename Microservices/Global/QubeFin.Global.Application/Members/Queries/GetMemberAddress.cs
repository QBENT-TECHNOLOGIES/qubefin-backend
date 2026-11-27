using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Members.Queries;

#region --- QUERY ---
public record GetMemberAddressesQuery(Guid MemberId) : IRequest<Result<List<MemberAddressResponse>>>;
#endregion

#region --- RESPONSE ---
public record MemberAddressResponse(
    Guid Id,
    Guid MemberId,
    string? ParmanentHouseNo,
    string? ParmanentStreetName,
    string? ParmanentCity,
    Guid ParmanentAdministrativeUnitId,
    Guid ParmanentPoliceStationId,
    string? ParmanentPostOffice,
    string? ParmanentPinCode,
    string? ParmanentNearestLandmark,
    int? ParmanentResidenceYear,
    bool PresentPermanentAddressSame,
    string? PresentHouseNo,
    string? PresentStreetName,
    string? PresentCity,
    Guid PresentAdministrativeUnitId,
    Guid PresentPoliceStationId,
    string? PresentPostOffice,
    string? PresentPinCode,
    string? PresentNearestLandmark,
    int? PresentResidenceYear,
    bool IsDefault,
    int Sequence
);
#endregion

#region --- HANDLER ---
internal sealed class GetMemberAddressesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMemberAddressesQuery, Result<List<MemberAddressResponse>>>
{
    public async Task<Result<List<MemberAddressResponse>>> Handle(
        GetMemberAddressesQuery request,
        CancellationToken cancellationToken)
    {
        var addresses = await context.TblMemberAddresses
            .AsNoTracking()
            .Where(a => a.MemberId == request.MemberId)
            .OrderBy(a => a.Sequence)
            .Select(a => new MemberAddressResponse(
                a.Id,
                a.MemberId,
                a.ParmanentHouseNo,
                a.ParmanentStreetName,
                a.ParmanentCity,
                a.ParmanentAdministrativeUnitId,
                a.ParmanentPoliceStationId,
                a.ParmanentPostOffice,
                a.ParmanentPinCode,
                a.ParmanentNearestLandmark,
                a.ParmanentResidenceYear,
                a.PresentPermanentAddressSame,
                a.PresentHouseNo,
                a.PresentStreetName,
                a.PresentCity,
                a.PresentAdministrativeUnitId,
                a.PresentPoliceStationId,
                a.PresentPostOffice,
                a.PresentPinCode,
                a.PresentNearestLandmark,
                a.PresentResidenceYear,
                a.IsDefault,
                a.Sequence
            ))
            .ToListAsync(cancellationToken);

        return Result.Ok(addresses);
    }
}
#endregion