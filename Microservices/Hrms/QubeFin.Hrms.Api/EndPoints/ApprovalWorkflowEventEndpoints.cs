using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.ApprovalWorkflows.Commands;
using QubeFin.Hrms.Application.ApprovalWorkflows.Queries;

namespace QubeFin.Hrms.Api.Endpoints;

public class ApprovalWorkflowEventEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("approval-workflow-events/tree", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetApprovalWorkflowEventTreeQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get approval workflow event tree")
        .WithDescription("Groups approval workflow events by category, leave type, salary grade, and organization unit type, with event details at each leaf.")
        .WithTags("Approval Workflow Events");

        app.MapGet("approval-workflow-events", async (string? category, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetApprovalWorkflowEventsQuery(category), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get approval workflow events")
        .WithDescription("Gets all approval workflow events, optionally filtered by category.")
        .WithTags("Approval Workflow Events");

        app.MapGet("approval-workflow-events/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetApprovalWorkflowEventByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get an approval workflow event by ID")
        .WithTags("Approval Workflow Events");

        app.MapPost("approval-workflow-events", async (CreateApprovalWorkflowEventsCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Create approval workflow events")
        .WithDescription("Creates multiple approval workflow events in one request.")
        .WithTags("Approval Workflow Events");

        app.MapPut("approval-workflow-events", async (UpdateApprovalWorkflowEventsCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Update approval workflow events")
        .WithDescription("Updates existing events and creates entries without an ID in one request.")
        .WithTags("Approval Workflow Events");
    }
}
