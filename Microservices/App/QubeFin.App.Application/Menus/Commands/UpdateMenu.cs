using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Menus.Commands;

#region --- COMMAND ---
public record UpdateMenuCommand(Guid Id, string Name, string Icon, string? Target, Guid ParentId, Guid UserId) : IRequest<Result<UpdateMenuResponse>>;
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
        var menu = await menuRepository.GetByIdAsync(request.Id);
        if (menu is null)
        {
            return new RecordNotFoundError($"Menu not found for the given Id");
        }

        menu.Update(request.Name, request.Icon, request.Target, request.ParentId, 0, request.UserId);
        menuRepository.Update(menu);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new UpdateMenuResponse(true));
    }
}
#endregion