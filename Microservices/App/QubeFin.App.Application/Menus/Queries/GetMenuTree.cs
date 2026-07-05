using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetMenuTreeQuery : IRequest<Result<List<MenuTree>>>;
#endregion

#region --- HANDLER ---
internal sealed class GetMenuTreeQueryHandler(IMenuRepository menuRepository)
    : IRequestHandler<GetMenuTreeQuery, Result<List<MenuTree>>>
{
    public async Task<Result<List<MenuTree>>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
    {
        var menus = await menuRepository.GetTreeAsync();
        return Result.Ok(MenuTreeBuilder.Build(menus));
    }
}
#endregion
