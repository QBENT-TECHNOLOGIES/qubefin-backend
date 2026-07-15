using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.Employees.Queries;
using System.Security.Claims;
using System.Security.Principal;

namespace QubeFin.Hrms.Api.Endpoints;

public class EmployeeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("employees/search", async (IMediator mediator, string searchType, string? searchText, Guid? searchOrganizationUnitId,
            string sortOn, string sortDirection, int pageIndex, int pageSize) =>
        {
            var resp = await mediator.Send(new GetEmployeesBySearchQuery(searchType, searchText, searchOrganizationUnitId,
                sortOn, sortDirection, pageIndex, pageSize));
            return TypedResults.Ok(resp);
        })
        .WithSummary("Search Employees by Free Text, Office Or Designation");

        app.MapGet("employees/{id:guid}", async (Guid id, ISender sender) =>
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

        app.MapPut("employees/update/personal/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id,  PersonalInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeePersonalCommand(id,request.Code, request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName,
                DateOnly.FromDateTime( request.DateOfBirth), request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisablityType, request.MaritalStatus, userId);
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
        .WithSummary("Update Employee Personal data");

        app.MapPut("employees/update/official/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, OfficialInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeOfficialCommand(id, request.CompanyId, request.OrganizationUnitId, request.DepartmentId, request.EmployementType, request.DateOfJoining, request.DateOfConfirmation,
                request.SeparationDate, request.ReferedBy, request.HowYouKnow, request.OfficialEmail, request.IsActive, userId);
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
        .WithSummary("Update Employee Official data");
    }
}
