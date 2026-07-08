using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Commands;

#region --- COMMAND ---
public record CreateMenuCommand(string Name, string Icon, string? Target, Guid ParentId, Guid UserId) : IRequest<Result<CreateMenuResponse>>;
#endregion

#region --- RESPONSE ---
public record CreateMenuResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class CreateMenuCommandHandler(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMenuCommand, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var maxPosition = await menuRepository.GetMaxMenuPositionAsync();
        var menu = Menu.Create(Guid.NewGuid(), request.Name, request.Icon, request.Target, request.ParentId, maxPosition + 1, request.UserId);
        await menuRepository.AddAsync(menu);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok(new CreateMenuResponse(true));
    }
}
#endregion