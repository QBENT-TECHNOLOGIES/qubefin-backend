using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Hrms.Application.Salaries.Commands;
using QubeFin.Hrms.Application.Salaries.Queries;
using QubeFin.Hrms.Application.Salaries.SalaryCategory.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints
{
    public class SalaryComponentEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {   
            app.MapGet("salary-components", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllSalaryComponentsQuery(), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get all salary components")
              .WithDescription("Retrieves a list of all salary components in the system.")
              .WithTags("Salary Components");
            app.MapGet("salary-components/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetSalaryComponentByIdQuery(id));
                return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
            }).WithSummary("Get a salary component by ID")
              .WithDescription("Retrieves a specific salary component by its unique identifier.")
              .WithTags("Salary Components");

            app.MapPost("salary-components", async (CreateSalaryComponentCommand command, ISender sender, ClaimsPrincipal principal) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();
                var commandWithUser = command with { CreatedBy = userId };
                var result = await sender.Send(commandWithUser);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Create a new salary component")
              .WithDescription("Creates a new salary component in the system.")
              .WithTags("Salary Components");

            app.MapPut("salary-components/{id}", async (Guid id, UpdateSalaryComponentCommand command, ISender sender, ClaimsPrincipal principal) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();
                var commandWithUser = command with { Id = id, LastModifiedBy = userId };
                var result = await sender.Send(commandWithUser);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Update an existing salary component")
              .WithDescription("Updates an existing salary component in the system.")
              .WithTags("Salary Components");
            app.MapGet("salary-components/categories", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllSalaryComponentCategoriesQuery(), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get all salary component categories")
              .WithDescription("Retrieves a list of all salary component categories in the system.")
              .WithTags("Salary Components");
        }
    }
}
