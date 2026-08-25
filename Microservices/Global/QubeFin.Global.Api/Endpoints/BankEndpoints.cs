using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Banks.Queries;

namespace QubeFin.Global.Api.Endpoints
{
    public class BankEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("banks", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllBankQuery(), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("GetAllBanks")
            .WithSummary("Get All Banks")
            .WithDescription("Returns all banks.")
            .WithTags("Bank")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
