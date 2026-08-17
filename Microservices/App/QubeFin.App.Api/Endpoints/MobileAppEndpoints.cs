using MediatR;
using QubeFin.App.Application.MobileApp.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

namespace QubeFin.App.Api.Endpoints;

public class MobileAppEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("mobile/latest-version/", async (ISender sender, string version) =>
        {
            var result = await sender.Send(new GetLatestVersionQuery(version));
            return result.ToHttpResult();
        }).WithSummary("Get Mobile Latest Version");
    }
}
