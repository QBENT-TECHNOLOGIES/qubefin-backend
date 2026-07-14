using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.OrganizationUnits.Type.Queries
{
    public record GetAllOrganizationUnitTypeQuery() : IRequest<Result<OrganizationUnitTypeResponse>>;
    public record OrganizationUnitTypeResponse(IEnumerable<OrganizationUnitType> OrganizationUnitTypes);
    internal sealed class GetAllOrganizationUnitTypeQueryHandler(IOrganizationUnitTypeRepository organizationUnitTypeRepository) : IRequestHandler<GetAllOrganizationUnitTypeQuery, Result<OrganizationUnitTypeResponse>>
    {
        public async Task<Result<OrganizationUnitTypeResponse>> Handle(GetAllOrganizationUnitTypeQuery request, CancellationToken cancellationToken)
        {
            var organizationUnitTypes = await organizationUnitTypeRepository.GetAllAsync();
            return Result.Ok(new OrganizationUnitTypeResponse(organizationUnitTypes));
        }
    }
}
