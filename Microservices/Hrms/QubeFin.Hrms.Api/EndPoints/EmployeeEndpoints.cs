using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Application.Employees.Queries;
using QubeFin.Persistence.Models.Hrms;
using System.Security.Claims;

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

        // ---------- START  GET BY ID -----------//
        app.MapGet("employees/personal-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeePersonalByIdQuery(id));
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
        .WithSummary("Get Employee Personal By Id");

        app.MapGet("employees/address-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeAddressByIdQuery(id));
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
        .WithSummary("Get Employee Address By Id");

        app.MapGet("employees/contact-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeContactByIdQuery(id));
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
        .WithSummary("Get Employee Contact Details By Id");

        app.MapGet("employees/official-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeOfficialByIdQuery(id));
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
        .WithSummary("Get Employee Official By Id");

        app.MapGet("employees/kyc-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeKycDetailQuery(id));
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
        .WithSummary("Get Employee KYC By Id");

        app.MapGet("employees/references/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeReferenceQuery(id));
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
        .WithSummary("Get Employee References By Id");
        

        // ---------- END  GET BY ID -----------//

        app.MapPut("employees/update/personal/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] PersonalInfoRequest request, ISender sender) =>
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

        app.MapPut("employees/update/official/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] OfficialInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeOfficialCommand(id, request.CompanyId, request.OrganizationUnitId, request.DepartmentId, request.EmployementType, request.DateOfJoining != null ? DateOnly.FromDateTime(request.DateOfJoining.Value) : null, request.DateOfConfirmation != null ? DateOnly.FromDateTime(request.DateOfConfirmation.Value) : null,
                request.SeparationDate != null ? DateOnly.FromDateTime(request.SeparationDate.Value) : null, request.ReferedBy, request.HowYouKnow, request.OfficialEmail, request.IsActive, userId);
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

        app.MapPut("employees/update/contact/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] ContactInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeContactCommand(id, request.MobileNo, request.PersonalEmail, request.PrimaryEmergencyRelation, request.PrimaryEmergencyName, request.PrimaryEmergencyMobile,
            request.SecondaryEmergencyRelation, request.SecondaryEmergencyName, request.SecondaryEmergencyMobile, userId);
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
        .WithSummary("Update Employee Contact data");

        app.MapPut("employees/update/address/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] AddressInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeAddressCommand(id, request?.PresentAddress, request?.PermanentAddress, userId);
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
        .WithSummary("Update Employee Address data");

        app.MapPut("employees/update/kyc/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] List<DocumentDetailRequest> Documents, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeDocumentCommand(id, Documents, userId);
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
        .WithSummary("Update Employee Address data");

        app.MapPut("employees/update/references/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] List<ReferenceDetailRequest> referenceDetail, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeReferenceCommand(id, referenceDetail, userId);
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
        .WithSummary("Update Employee Address data");

        app.MapPut("employees/update/banking-info/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] BankDetail request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeePayrollCommand(id, request.BankId, request.BankAccountNo, request.BankHolderName, request.BankBranch, request.BankAccountType, request.HasEsiEligible,
                request.EsiIpNumber, request.UniversalAccountNumber, request.IsPayrollActive, userId);
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
        .WithSummary("Update Banking Info data");

        app.MapPost("employees/search-by-text", async (IMediator mediator, SearchTextRequest request) =>
        {
            var resp = await mediator.Send(new GetEmployeeBySearchTextQuery(request.SearchText));
            return TypedResults.Ok(resp);
        }).WithSummary("Search Employees by Text");
    }
}
