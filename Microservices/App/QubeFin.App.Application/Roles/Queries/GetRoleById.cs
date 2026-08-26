using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Roles.Queries;

#region --- QUERY ---
public record GetRoleByIdQuery(Guid Id) : IRequest<Result<Role>>;
#endregion
#region --- HANDLER ---
internal sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRoleByIdQuery, Result<Role>>
{
    public async Task<Result<Role>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role is null)
        {
            return new RecordNotFoundError($"Role not found for the given Id");
        }
        return Result.Ok(role);
    }
}
#endregion
