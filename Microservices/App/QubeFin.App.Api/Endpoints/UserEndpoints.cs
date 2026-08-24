using MediatR;
using QubeFin.App.Api.Requests;
using QubeFin.App.Application.Users.Commands;
using QubeFin.App.Application.Users.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using System.Security.Claims;

namespace QubeFin.App.Api.Endpoints;

public class UserEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (ISender sender) =>
        {
            var result = await sender.Send(new GetUsersQuery());
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Users");

        app.MapGet("users/search", async (ISender sender, string? searchText, string sortOn, string sortDirection, int pageIndex, int pageSize) =>
        {
            var result = await sender.Send(new GetUsersBySearchQuery(searchText, sortOn, sortDirection, pageIndex, pageSize));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithSummary("Search Users by User Name or Employee Name");

        app.MapGet("users/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get User By Id");

        app.MapGet("users/login-info", async (ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var employeeId = principal.Identity.GetEmployeeId();

            var result = await sender.Send(new GetUserLoginInfoQuery(userId, employeeId));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Logeedin User Info");

        app.MapPost("users", async (ClaimsPrincipal principal, UserCreateRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateUserCommand(request.UserName, request.Password, request.EmployeeId, principal.Identity.GetUserId()));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create User");

        app.MapPut("users/{id:guid}", async (ClaimsPrincipal principal, UserUpdateRequest request, ISender sender,Guid id) =>
        {
            var result = await sender.Send(new UpdateUserCommand(id, request.UserName, request.IsActive, request.HasMfaEnabled, principal.Identity.GetUserId()));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Update User");

        app.MapPost("register-mfa", async (RegisterMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            return result.ToHttpResult();
        });

        app.MapPost("enable-mfa", async (EnableMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            return result.ToHttpResult();
        });
    }
}
