using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Application.Employees.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class EmployeeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("employees/search", async (IMediator mediator, string searchType, string? searchText, DateOnly? srchJoiningDate, Guid? searchOrganizationUnitId,
            string sortOn, string sortDirection, int pageIndex, int pageSize) =>
        {
            var result = await mediator.Send(new GetEmployeesBySearchQuery(searchType, searchText, srchJoiningDate, searchOrganizationUnitId,
                sortOn, sortDirection, pageIndex, pageSize));
            return TypedResults.Ok(result);
        })
        .WithSummary("Search Employees by Free Text, Office Or Designation")
        .RequireAuthorization();

        app.MapGet("employees/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee By Id")
        .RequireAuthorization();

        app.MapPost("employees", async (CreateEmployeeCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Create Employee")
        .RequireAuthorization();

        // ---------- START  GET BY ID -----------//
        app.MapGet("employees/personal-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeePersonalByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Personal By Id")
        .RequireAuthorization();

        app.MapGet("employees/address-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeAddressByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Address By Id")
        .RequireAuthorization();

        app.MapGet("employees/contact-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeContactByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Contact Details By Id")
        .RequireAuthorization();

        app.MapGet("employees/official-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeOfficialByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Official By Id")
        .RequireAuthorization();

        app.MapGet("employees/kyc-details/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeKycDetailQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee KYC By Id")
        .RequireAuthorization();

        app.MapGet("employees/references/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeReferenceQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee References By Id")
        .RequireAuthorization();

        app.MapGet("employees/employments/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeEmploymentQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Employments By Id")
        .RequireAuthorization();

        app.MapGet("employees/qualifications/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeQualificationQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Qualifications By Id")
        .RequireAuthorization();

        app.MapGet("employees/banking/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeePayrollByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Employee Banking Info By Id")
        .RequireAuthorization();


        // ---------- END  GET BY ID -----------//

        app.MapPut("employees/update/personal/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] PersonalInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeePersonalCommand(id,request.Code, request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName,
                request.DateOfBirth, request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisablityType, request.MaritalStatus, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Personal data")
        .RequireAuthorization();

        app.MapPut("employees/update/official/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] OfficialInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeOfficialCommand(id, request, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Official data")
        .RequireAuthorization();

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
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Contact data")
        .RequireAuthorization();

        app.MapPut("employees/update/address/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] AddressInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeAddressCommand(id, request?.PresentAddress, request?.PermanentAddress, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Address data")
        .RequireAuthorization();

        app.MapPut("employees/update/kyc/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, HttpRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            if (!request.HasFormContentType) return Results.BadRequest("Invalid content type");

            var form = await request.ReadFormAsync();
            var documents = new List<DocumentDetailRequest>();
            int index = 0;
            while (form.ContainsKey($"documents[{index}].documentName"))
            {
                var doc = new DocumentDetailRequest
                {
                    DocumentName = form[$"documents[{index}].documentName"].ToString(),
                    DocumentNo = form[$"documents[{index}].documentNo"].ToString(),
                    FileName = form[$"documents[{index}].fileName"].ToString(),
                    File = form.Files[$"documents[{index}].file"] 
                };

                if (DateTime.TryParse(form[$"documents[{index}].validFrom"], out var validFrom))
                    doc.ValidFrom = validFrom;

                if (DateTime.TryParse(form[$"documents[{index}].validTill"], out var validTill))
                    doc.ValidTill = validTill;

                documents.Add(doc);
                index++;
            }

            var command = new UpdateEmployeeDocumentCommand(id, documents, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
            //var command = new UpdateEmployeeDocumentCommand(id, Documents, userId);
            //var result = await sender.Send(command);
            //return result.ToHttpResult();
        })
        .WithSummary("Update Employee Kyc data")
        .RequireAuthorization();

        app.MapPatch("employees/update/references/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] List<ReferenceDetailRequest> referenceDetail, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeReferenceCommand(id, referenceDetail, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Reference data")
        .RequireAuthorization();

        app.MapPatch("employees/update/employments/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] List<EmploymentDetailRequest> employments, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeEmploymentCommand(id, employments, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Employment data")
        .RequireAuthorization();

        app.MapPatch("employees/update/qualifications/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] List<QualificationRequest> employments, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeeQualificationCommand(id, employments, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Employee Qualification data")
        .RequireAuthorization();

        app.MapPatch("employees/update/banking-info/{id:guid}", async (ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] BankDetail request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var command = new UpdateEmployeePayrollCommand(id, request.BankId, request.BankAccountNo, request.BankHolderName, request.BankBranch, request.BankAccountType, request.HasEsiEligible,
                request.EsiIpNumber, request.UniversalAccountNumber, request.IsPayrollActive, userId);
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Banking Info data")
        .RequireAuthorization();

        app.MapPost("employees/search-by-text", async (IMediator mediator, SearchTextRequest request) =>
        {
            var result = await mediator.Send(new GetEmployeeBySearchTextQuery(request.SearchText));
            return result.ToHttpResult();
        })
        .WithSummary("Search Employees by Text")
        .RequireAuthorization();
    }
}
