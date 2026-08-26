using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Roles.Commands;

#region --- COMMAND ---
public record UpdateRoleCommand(Guid Id, string Name, bool IsActive, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- HANDLER ---
internal sealed class UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role == null)
        {
            return Result.Fail($"Role with Id {request.Id} not found");
        }
        role.Update(request.Name, request.IsActive, request.UserId);
        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok($"Role {request.Name} updated successfully");
    }
}
#endregion
