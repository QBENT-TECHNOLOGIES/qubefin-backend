using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnis.Commands;

#region --- COMMAND ---
public record CreateOrganizationUnitCommand(Guid OrganizationUnitTypeId, string Name, int CodeVal, Guid? ParentId,
        TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, Guid UserId) : IRequest<Result<CreateOrganizationUnitResponse>>;
#endregion

#region --- RESPONSE ---
public record CreateOrganizationUnitResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class CreateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) : 
    IRequestHandler<CreateOrganizationUnitCommand, Result<CreateOrganizationUnitResponse>>
{
    public async Task<Result<CreateOrganizationUnitResponse>> Handle(CreateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = OrganizationUnit.Create(Guid.NewGuid(), request.OrganizationUnitTypeId, request.Name, request.CodeVal, request.ParentId,
            request.AttendanceInTime, request.AttendanceOutTime, request.UserId);
        await organizationUnitRepository.AddAsync(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateOrganizationUnitResponse(true));
    }
}
#endregion