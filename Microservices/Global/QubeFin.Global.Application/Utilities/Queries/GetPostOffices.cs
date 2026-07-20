using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnitTypes.Queries;

#region --- QUERY ---
public record GetPostOfficesQuery : IRequest<Result<List<GetPostOfficeResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetPostOfficeResponse(Guid Id, string Name);
#endregion

#region --- HANDLER ---
internal sealed class GetPostOfficesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetPostOfficesQuery, Result<List<GetPostOfficeResponse>>>
{
    public async Task<Result<List<GetPostOfficeResponse>>> Handle(GetPostOfficesQuery request, CancellationToken cancellationToken)
    {
        var utilities = await context
            .TblPostOffices
            .OrderBy(a => a.Name)
            .Select(a => new GetPostOfficeResponse(a.Id, a.Name))
            .ToListAsync(cancellationToken);

        return Result.Ok(utilities);
    }
}
#endregion
