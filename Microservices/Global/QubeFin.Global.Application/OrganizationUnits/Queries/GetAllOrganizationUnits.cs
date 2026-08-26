using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

internal class GetAllOrganizationUnits
{
}


#region --- QUERY ---
public record GetAllOrganizationUnitsQuery() : IRequest<Result<List<GetAllOrganizationUnitsResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetAllOrganizationUnitsResponse(Guid Id, string Name);
#endregion

#region --- HANDLER ---
internal sealed class GetAllOrganizationUnitsQueryHandler(IOrganizationUnitRepository OrganizationUnitRepository)
    : IRequestHandler<GetAllOrganizationUnitsQuery, Result<List<GetAllOrganizationUnitsResponse>>>
{
    public async Task<Result<List<GetAllOrganizationUnitsResponse>>> Handle(GetAllOrganizationUnitsQuery request, CancellationToken cancellationToken)
    {
        var organizationUnits = await OrganizationUnitRepository.GetAllAsync(cancellationToken);
        return Result.Ok(organizationUnits.Select(m => new GetAllOrganizationUnitsResponse(m.Id, m.Name)).ToList());
    }
}
#endregion