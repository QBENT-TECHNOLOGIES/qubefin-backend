using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnitTypes.Queries;

#region --- QUERY ---
public record GetUtilityQuery : IRequest<Result<List<GetUtilityResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetUtilityResponse(string SysKey, string SysVal);
#endregion

#region --- HANDLER ---
internal sealed class GetUtilityQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUtilityQuery, Result<List<GetUtilityResponse>>>
{
    public async Task<Result<List<GetUtilityResponse>>> Handle(GetUtilityQuery request, CancellationToken cancellationToken)
    {
        var utilities = await context
            .TblSystemValues
            .OrderBy(a => a.SysKey)
            .Select(a => new GetUtilityResponse(a.SysKey, a.SysVal))
            .ToListAsync(cancellationToken);

        return Result.Ok(utilities);
    }
}
#endregion
