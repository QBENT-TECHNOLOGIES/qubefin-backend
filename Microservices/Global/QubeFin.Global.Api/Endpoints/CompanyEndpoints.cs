using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Companies.Queries;

namespace QubeFin.Global.Api.Endpoints
{
    public class CompanyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("companies", async(ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllCompanyQuery(), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("GetAllCompanies")
            .WithSummary("Get All Companies")
            .WithDescription("Returns the all company name.")
            .WithTags("Company")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
