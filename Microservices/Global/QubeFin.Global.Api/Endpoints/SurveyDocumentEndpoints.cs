using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyDocuments.Commands;
using QubeFin.Global.Application.SurveyDocuments.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyDocumentEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/documents", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetSurveyDocumentsQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Survey Photographs")
            .WithTags("SurveyDocuments")
            .RequireAuthorization();

            // Multipart upload, same as the other upload endpoints in the project.
            app.MapPost("surveys/{id:guid}/documents", async (Guid id, IFormFile file, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();

                await using var fileStream = file.OpenReadStream();
                var command = new UploadSurveyDocumentCommand(id, fileStream, file.FileName, file.ContentType, file.Length, userId, employeeId);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .DisableAntiforgery()
            .WithSummary("Upload A Survey Photograph")
            .WithTags("SurveyDocuments")
            .RequireAuthorization();

            app.MapDelete("surveys/{id:guid}/documents/{documentId:guid}", async (Guid id, Guid documentId, ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new DeleteSurveyDocumentCommand(id, documentId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Remove A Survey Photograph")
            .WithTags("SurveyDocuments")
            .RequireAuthorization();
        }
    }
}
