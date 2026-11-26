using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Groups.Queries;

#region --- QUERY ---
public record GetUserWiseGroupBySearchTextQuery(Guid EmployeeId, string? SearchText)
    : IRequest<Result<List<GetUserWiseGroupBySearchTextResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetUserWiseGroupBySearchTextResponse(Guid Id, string? GroupCode, string Name);
#endregion

#region --- HANDLER ---
internal sealed class GetUserWiseGroupBySearchTextQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUserWiseGroupBySearchTextQuery, Result<List<GetUserWiseGroupBySearchTextResponse>>>
{
    public async Task<Result<List<GetUserWiseGroupBySearchTextResponse>>> Handle(
        GetUserWiseGroupBySearchTextQuery request,
        CancellationToken cancellationToken)
    {
        var searchText = request.SearchText?.Trim();

        var groupEntities = await context.TblGroups
            .AsNoTracking()
            .Include(g => g.Designation)
            .ThenInclude(d => d!.TblEmployeeDesignations)
            .Where(g => g.Designation != null
                && g.Designation.TblEmployeeDesignations.Any(ed =>
                    ed.EmployeeId == request.EmployeeId &&
                    ed.EffectiveTo == null)
                && (string.IsNullOrEmpty(searchText)
                    || (g.Code != null && g.Code.Contains(searchText))
                    || g.Name.Contains(searchText)))
            .OrderBy(g => g.Name)
            .Take(10)
            .ToListAsync(cancellationToken);

        var searchResult = groupEntities
            .Select(g => new GetUserWiseGroupBySearchTextResponse(
                g.Id,
                g.Code,
                g.Name))
            .ToList();

        return Result.Ok(searchResult);
    }
}
#endregion