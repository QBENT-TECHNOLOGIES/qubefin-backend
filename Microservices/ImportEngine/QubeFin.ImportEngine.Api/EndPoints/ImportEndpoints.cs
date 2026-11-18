using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.ImportEngine.Application.Imports.Commands;
using QubeFin.ImportEngine.Application.Imports.Queries;
using System.Security.Claims;

namespace QubeFin.ImportEngine.Api.EndPoints;

public class ImportEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("import-types", async (ISender sender) =>
        {
            var result = await sender.Send(new GetActiveImportTypesQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get active import types")
        .WithTags("Imports")
        .RequireAuthorization();

        app.MapPost("import", async (ClaimsPrincipal principal, [FromForm] IFormFile file, [FromForm] Guid importTypeId, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var result = await sender.Send(new CreateImportJobCommand(file, importTypeId, userId));
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Upload an Excel file and run its import")
        .WithTags("Imports")
        .RequireAuthorization();

        app.MapGet("import-jobs/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetImportJobByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get an import job by id")
        .WithTags("Imports")
        .RequireAuthorization();
    }
}
