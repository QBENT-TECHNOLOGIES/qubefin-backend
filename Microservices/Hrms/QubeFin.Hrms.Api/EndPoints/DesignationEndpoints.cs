using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Designations.Queries;

namespace QubeFin.Hrms.Api.EndPoints;

public class DesignationEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("designations/{id:guid}/organization-unit", async (ISender sender, [FromRoute] Guid id) =>
        {
            var result = await sender.Send(new GetAllByOrganizationUnitQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get All Designation By Organization Unit")
        .WithTags("Designations")
        .RequireAuthorization();
    }
}