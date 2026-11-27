using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Api.Requests;
using QubeFin.Global.Application.Members.Commands;
using QubeFin.Global.Application.Members.Models;
using QubeFin.Global.Application.Members.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints;

public class MemberEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("members/search", async (ISender sender, MemberSearchParam searchParam) =>
        {
            var result = await sender.Send(new GetMembersBySearchQuery(searchParam));
            return TypedResults.Ok(result);
        })
        .WithSummary("Search Members ")
        .RequireAuthorization();

        app.MapGet("members/personal-details/{id:guid}", async ([FromRoute] Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMemberPersonalDetailsQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Member Personal Details")
        .RequireAuthorization();

        app.MapPost("members", async (ClaimsPrincipal principal, PersonalInfoRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var employeeId = principal.Identity.GetEmployeeId();

            var command = new CreateMemberCommand(
                request.BranchCode,
                request.GroupId,
                request.Salutaion,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.GuardianName,
                request.MotherName,
                request.SpouseName,
                request.DateOfBirth,
                request.Gender,
                request.Religion,
                request.Caste,
                request.Nationality,
                request.MaritalStatus,
                request.EducationalQualification,
                request.MobileNo,
                request.AlternateMobileNo,
                userId,
                employeeId);

            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Create Member")
        .RequireAuthorization();

        app.MapPut("members/update/personal/{id:guid}", async ( ClaimsPrincipal principal, [FromRoute] Guid id, PersonalInfoRequest request,  ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();

            var command = new UpdateMemberPersonalCommand(
                id,
                request.BranchCode,
                request.GroupId,
                request.Salutaion,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.GuardianName,
                request.MotherName,
                request.SpouseName,
                request.DateOfBirth,
                request.Gender,
                request.Religion,
                request.Caste,
                request.Nationality,
                request.MaritalStatus,
                request.EducationalQualification,
                request.MobileNo,
                request.AlternateMobileNo,
                userId);

            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Member Personal data")
        .RequireAuthorization();

        app.MapGet("members/addresses/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMemberAddressesQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Member Addresses")
        .WithTags("Members")
        .RequireAuthorization();

        app.MapPut("members/addresses/{id:guid}", async (
            Guid id,
            List<MemberAddressRequest> request,
            ClaimsPrincipal principal,
            ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateMemberAddressCommand(id, request, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Update Member Addresses")
        .WithTags("Members")
        .RequireAuthorization();

        app.MapGet("members/documents/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMemberDocumentsQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Member KYC Documents")
        .WithTags("Members")
        .RequireAuthorization();

        app.MapPut("members/documents/{id:guid}", async (
            Guid id,
            [FromForm] MemberDocumentsFormRequest request,
            ClaimsPrincipal principal,
            ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateMemberDocumentCommand(
                id,
                request.Documents,
                userId));
            return result.ToHttpResult();
        })
        .WithSummary("Update Member KYC Documents")
        .WithTags("Members")
        .DisableAntiforgery()
        .RequireAuthorization();
    }
}