using Azure.Core;
using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Commands;
using QubeFin.Hrms.Application.ApprovalWorkflows.Models;
using QubeFin.Hrms.Application.ApprovalWorkflows.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class ApprovalWorkflowEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("approval-workflows", async (string? category, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetApprovalWorkflowsQuery(category), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization()
        .WithSummary("Get approval workflows")
        .WithTags("Approval Workflows");

        app.MapPost("approval-workflows/search", async (ApprovalWorkflowSearchRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new SearchApprovalWorkflowQuery(request),
                cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization()
        .WithSummary("Search approval workflows")
        .WithDescription("Searches approval workflows by category and organization unit type.")
        .WithTags("Approval Workflows");

        app.MapGet("approval-workflows/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetApprovalWorkflowByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization()
        .WithSummary("Get an approval workflow by ID")
        .WithTags("Approval Workflows");

        app.MapPost("approval-workflows", async (ApprovalWorkflowRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var result = await sender.Send(new CreateApprovalWorkflowCommand(request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization()
        .WithSummary("Create an approval workflow")
        .WithTags("Approval Workflows");

        app.MapPut("approval-workflows/{id:guid}", async (Guid id, ApprovalWorkflowRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateApprovalWorkflowCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization()
        .WithSummary("Update an approval workflow")
        .WithTags("Approval Workflows");
    }
}
