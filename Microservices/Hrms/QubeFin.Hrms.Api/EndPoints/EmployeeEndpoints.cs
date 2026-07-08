using FluentResults;
using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.Employees.Queries;

namespace QubeFin.Hrms.Api.Endpoints;

public class EmployeeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
      

        app.MapPost("employees", async (CreateEmployeeCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            if (result.IsFailed)
            {
                if (result.Errors[0] is RecordNotFoundError)
                {
                    return Results.NotFound(result.Errors[0]);
                }
                if (result.Errors[0] is ValidationError)
                {
                    return Results.BadRequest(result.Errors[0]);
                }
            }
            return Results.Ok();
        })
        .WithSummary("Create Employee");

        app.MapPost("employees/update", async (UpdateEmployeeCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            if (result.IsFailed)
            {
                if (result.Errors[0] is RecordNotFoundError)
                {
                    return Results.NotFound(result.Errors[0]);
                }
                if (result.Errors[0] is ValidationError)
                {
                    return Results.BadRequest(result.Errors[0]);
                }
            }
            
            return Results.Ok();
        })
        .WithSummary("Update Employee");

        app.MapGet("employees/getById/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeByIdQuery(id));
            if (result.IsFailed)
            {
                if (result.Errors[0] is RecordNotFoundError)
                {
                    return Results.NotFound(result.Errors[0]);
                }
                if (result.Errors[0] is ValidationError)
                {
                    return Results.BadRequest(result.Errors[0]);
                }
            }
            return Results.Ok(result.Value);
        })
        .WithSummary("Get Employee By Id");

        app.MapGet("employees", async (ISender sender) =>
        {
            var employee = await sender.Send(new GetAllEmployeeQuery());
            return Results.Ok(employee.Value);
        })
        .WithSummary("Get All Employee");

    }
}
