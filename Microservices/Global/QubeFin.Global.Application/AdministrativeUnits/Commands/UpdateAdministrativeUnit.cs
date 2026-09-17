using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Commands;

#region --- COMMAND ---
public record UpdateAdministrativeUnitCommand(Guid Id, Guid AdministrativeUnitTypeId, string Name, Guid? ParentId, bool IsActive, Guid UserId) : IRequest<Result<UpdateAdministrativeUnitResponse>>;
#endregion

#region --- RESPONSE ---
public record UpdateAdministrativeUnitResponse(bool Updated);
#endregion

#region --- HANDLER ---
internal sealed class UpdateAdministrativeUnitCommandHandler(IAdministrativeUnitRepository administrativeUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAdministrativeUnitCommand, Result<UpdateAdministrativeUnitResponse>>
{
    public async Task<Result<UpdateAdministrativeUnitResponse>> Handle(UpdateAdministrativeUnitCommand request, CancellationToken cancellationToken)
    {
        var administrativeUnit = await administrativeUnitRepository.GetByIdAsync(request.Id);
        if (administrativeUnit is null)
        {
            return new RecordNotFoundError($"Administrative Unit not found for the given Id");
        }

        if (request.ParentId.HasValue)
        {
            var allUnits = await administrativeUnitRepository.GetAllAsync(cancellationToken);
            if (CreatesCycle(request.Id, request.ParentId.Value, allUnits))
            {
                return new ValidationError("An Administrative Unit cannot be placed under itself or under one of its own child units");
            }
        }

        administrativeUnit.Update(request.AdministrativeUnitTypeId, request.Name, request.ParentId, request.IsActive, request.UserId);
        administrativeUnitRepository.Update(administrativeUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateAdministrativeUnitResponse(true));
    }

    /// <summary>
    /// Walks up from the proposed parent. Reaching the unit being updated means the new parent is
    /// the unit itself or one of its descendants, which would detach the branch from the tree and
    /// make the hierarchy query recurse forever.
    /// </summary>
    private static bool CreatesCycle(Guid id, Guid parentId, IEnumerable<AdministrativeUnitTree> units)
    {
        var parents = units.ToDictionary(unit => unit.Id, unit => unit.ParentId);

        Guid? current = parentId;
        while (current.HasValue)
        {
            if (current.Value == id)
            {
                return true;
            }

            if (!parents.TryGetValue(current.Value, out var next))
            {
                return false;
            }

            current = next;
        }

        return false;
    }
}
#endregion