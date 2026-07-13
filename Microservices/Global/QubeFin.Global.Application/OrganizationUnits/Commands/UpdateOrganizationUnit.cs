using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.OrganizationUnits.Commands
{
    public record UpdateOrganizationCommand(Guid id, Guid organizationUnitTypeId,
            string name,
            int codeVal,
            Guid? parentId,
            Guid? lastModifiedBy,
            TimeOnly? attendanceInTime,
            TimeOnly? attendanceOutTime
        ) : IRequest<Result<UpdateOrganizationUnitResponse>>;
    public record UpdateOrganizationUnitResponse(bool update);
    internal class UpdateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrganizationCommand, Result<UpdateOrganizationUnitResponse>>
    {
        public async Task<Result<UpdateOrganizationUnitResponse>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            //var existingEntity = await organizationUnitRepository.GetByIdAsync(request.id);
            //if (existingEntity is null) return new RecordNotFoundError("Organization unit not found");
            //existingEntity.Update(
            //    organizationUnitTypeId: request.organizationUnitTypeId,
            //    name: request.name,
            // codeVal: request.codeVal,
            //  parentId: request.parentId,
            //  lastModifiedBy: request.lastModifiedBy,
            // attendanceInTime: request.attendanceInTime,
            // attendanceOutTime: request.attendanceOutTime
            //    );
            //await organizationUnitRepository.UpdateAsync(existingEntity);
            //await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateOrganizationUnitResponse(true));
        }
    }
}
