using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Roles.Commands;

#region --- COMMAND ---
public record CreateRoleCommand(string Name, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- HANDLER ---
internal sealed class CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = Role.Create(Guid.NewGuid(), request.Name, request.UserId);
        roleRepository.AddAsync(role);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok($"Role {request.Name} created successfully");
    }
}
#endregion
