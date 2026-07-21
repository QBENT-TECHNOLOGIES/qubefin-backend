using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.Companies.Queries;
using QubeFin.Global.Application.OrganizationUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints
{
    public class CompanyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("companies", async(ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllCompanyQuery(), cancellationToken);
                if (result.IsFailed)
                {
                    return Results.Problem("Failed to retrieve Organization Unit tree");
                }
                return Results.Ok(result.Value);
            }).WithName("GetAllCompanies")
            .WithSummary("Get All Companies")
            .WithDescription("Returns the all company name.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
