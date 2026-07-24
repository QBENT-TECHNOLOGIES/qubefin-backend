using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.App.Application.Menus.Commands;

#region --- COMMAND ---
public record UpdateMenuCommand(Guid Id, string Name, string Icon, string? Target, Guid ParentId, Guid UserId, List<Permission> Permissions) : IRequest<Result<UpdateMenuResponse>>;
#endregion

#region --- RESPONSE ---
public record UpdateMenuResponse(bool Updated);
#endregion

#region --- HANDLER ---
internal sealed class UpdateMenuCommandHandler(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMenuCommand, Result<UpdateMenuResponse>>
{
    public async Task<Result<UpdateMenuResponse>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = new Menu(request.Id, request.Name, request.Icon, request.Target, request.ParentId, request.UserId, request.Permissions);

        await menuRepository.UpdateAsync(menu, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new UpdateMenuResponse(true));
    }
}
#endregion