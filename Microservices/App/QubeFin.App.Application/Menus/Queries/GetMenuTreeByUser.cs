using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetMenuTreeByUserQuery(Guid EmployeeId) : IRequest<Result<List<MenuTree>>>;
#endregion

#region --- HANDLER ---
internal sealed class GetMenuTreeByUserQueryHandler(IMenuRepository menuRepository)
    : IRequestHandler<GetMenuTreeByUserQuery, Result<List<MenuTree>>>
{
    public async Task<Result<List<MenuTree>>> Handle(GetMenuTreeByUserQuery request, CancellationToken cancellationToken)
    {
        var menus = await menuRepository.GetTreeAsync(request.EmployeeId);
        return Result.Ok(MenuTreeBuilder.Build(menus));
    }
}
#endregion
