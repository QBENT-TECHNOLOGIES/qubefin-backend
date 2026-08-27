using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Departments.Commands;
using QubeFin.Hrms.Application.Departments.Models;
using QubeFin.Hrms.Application.Departments.Queries;
using QubeFin.Hrms.Application.Salaries.Commands;
using QubeFin.Hrms.Application.Salaries.Queries;
using System.Security.Claims;
namespace QubeFin.Hrms.Api.EndPoints;

public class DepartmentEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {

        app.MapGet("departments", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllDepartmentQuery(), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Get all departments")
            .WithDescription("Retrieves a list of departments.")
            .WithTags("Departmentss")
          .RequireAuthorization();

        app.MapGet("departments/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentByIdQuery(id));
            return result.ToHttpResult();
        }).WithSummary("Get a department by ID")
          .WithDescription("Retrieves a specific department by its unique identifier.")
          .WithTags("Departments")
        .RequireAuthorization();

        app.MapPost("departments", async (ISender sender, ClaimsPrincipal principal, DepartmentRequest request) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CreateDepartmentCommand(request.Name, request.IsActive, userId));
            return result.ToHttpResult();
        }).WithSummary("Create a new department")
          .WithDescription("Creates a new department in the system.")
          .WithTags("Department")
        .RequireAuthorization();

        app.MapPut("departments/{id}", async (Guid id, DepartmentRequest request, ISender sender, ClaimsPrincipal principal) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateDepartmentCommand(id, request.Name, request.IsActive, userId));
            return result.ToHttpResult();
        }).WithSummary("Update an existing department")
          .WithDescription("Updates an existing department in the system.")
          .WithTags("Departments")
        .RequireAuthorization();
    }
}