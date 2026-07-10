using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnis.Commands
{
    public record CreateOrganizationUnitCommand(Guid organizationUnitTypeId,
            string name,
            int codeVal,
            Guid? parentId,
            Guid createdBy,
            TimeOnly? attendanceInTime,
            TimeOnly? attendanceOutTime) : IRequest<Result<CreateOrganizationUnitResponse>>;
    public record CreateOrganizationUnitResponse(bool Created);
    internal sealed class CreateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) : 
        IRequestHandler<CreateOrganizationUnitCommand, Result<CreateOrganizationUnitResponse>>
    {
        public async Task<Result<CreateOrganizationUnitResponse>> Handle(CreateOrganizationUnitCommand request, CancellationToken cancellationToken)
        {
            var organizationUnitEntity = OrganizationUnit.Create(
                Guid.NewGuid(),
                request.organizationUnitTypeId,
                request.name,
                request.codeVal,
                request.parentId,
                request.createdBy,
                request.attendanceInTime,
                request.attendanceOutTime
                );
            await organizationUnitRepository.CreateAsync( organizationUnitEntity );
            await unitOfWork.SaveChangesAsync( cancellationToken );
            return Result.Ok(new CreateOrganizationUnitResponse(true));
        }
    }
}
