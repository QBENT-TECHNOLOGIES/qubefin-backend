using FluentResults;
using MediatR;
using QubeFin.Global.Application.Companies.Queries;
using QubeFin.Global.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

public record GetAllPostofficeQuery() : IRequest<Result<List<GetAllPostofficeResponse>>>;
public record GetAllPostofficeResponse(Guid Id, string Name, string pincode);

internal sealed class GetAllPostofficeQueryHandler(IAdministrativeUnitRepository administrativeUnit): IRequestHandler<GetAllPostofficeQuery, Result<List<GetAllPostofficeResponse>>>
{
    public async Task<Result<List<GetAllPostofficeResponse>>> Handle(GetAllPostofficeQuery request, CancellationToken cancellationToken)
    {
        var postoffices = await administrativeUnit.GetAllPostofficeAsync();
        return Result.Ok(postoffices.Select(m=>new GetAllPostofficeResponse(m.Id, m.Name, m.Pincode)).ToList());
    }
}
