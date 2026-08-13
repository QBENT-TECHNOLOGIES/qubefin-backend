using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.OrganizationUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class UtilityEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("utilities", async (ISender sender) =>
        {
            var user = await sender.Send(new GetUtilityQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Utility").WithTags("Utilities");

        app.MapGet("police-stations", async (ISender sender) =>
        {
            var user = await sender.Send(new GetPoliceStationsQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Police Station").WithTags("Utilities");

        app.MapGet("post-office", async (ISender sender) =>
        {
            var user = await sender.Send(new GetPostOfficesQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Post Office").WithTags("Utilities");

        app.MapGet("kyc-documents", async (ISender sender) =>
        {
            var user = await sender.Send(new GetKycDocumentsQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get KYC Documents").WithTags("Utilities");
    }
}
