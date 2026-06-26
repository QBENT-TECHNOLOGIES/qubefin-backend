using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

#region --- QUERY ---
public record GetAdministrativeUnitByIdQuery(Guid Id) : IRequest<Result<GetAdministrativeUnitByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetAdministrativeUnitByIdResponse(Guid Id, string Name, Guid AdministrativeUnitTypeId, string AdministrativeUnitType, Guid? ParentId);
#endregion

#region --- HANDLER ---
internal sealed class GetAdministrativeUnitByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAdministrativeUnitByIdQuery, Result<GetAdministrativeUnitByIdResponse>>
{
    public async Task<Result<GetAdministrativeUnitByIdResponse>> Handle(GetAdministrativeUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context
            .TblAdministrativeUnits
            .Include(m => m.AdministrativeUnitType)
            .Where(m => m.Id == request.Id)
            .Select(m => new GetAdministrativeUnitByIdResponse(m.Id, m.Name, m.AdministrativeUnitTypeId, m.AdministrativeUnitType.Name, m.ParentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return new RecordNotFoundError($"Administrative Unit not found for the given Id");
        }
        return Result.Ok(user);
    }
}
#endregion
