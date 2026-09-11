using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Auth.Application.Accounts.Commands;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Auth.Application.Accounts.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using System.Security.Claims;
using System.Security.Principal;

namespace QubeFin.Auth.Api.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("validate-login", async (HttpContext httpContext, ValidateLoginCommand request, [FromHeader(Name = "X-Device-Id")] string? deviceId, ISender sender) =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            request = request with
            {
                DeviceId = deviceId,
                UserAgent = userAgent
            };

            var result = await sender.Send(request);
            return result.ToHttpResult();
        });

        app.MapPost("verify-mfa", async (VerifyMfaCommand request, ISender sender) =>
        {
            var result = await sender.Send(request);
            return result.ToHttpResult();
        });

        app.MapGet("refresh-token", async ([FromHeader(Name = "Refresh-Token")] string refreshToken, ISender sender) =>
        {
            var result = await sender.Send(new ValidateRefreshTokenQuery(refreshToken));
            return result.ToHttpResult();
        });

        app.MapPost("change-password", async (ClaimsPrincipal principal, ChangePasswordRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new ChangePasswordCommand(request, userId));
            return result.ToHttpResult();
        }).RequireAuthorization();

        #region FORGOT PASSWORD

        app.MapPost("forgot-password", async (HttpContext httpContext, ForgotPasswordInitiateRequest request, [FromHeader(Name = "X-Device-Id")] string ? deviceId, ISender sender) =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            request.DeviceId = deviceId;
            request.UserAgent = userAgent;

            var result = await sender.Send(new ForgotPasswordCommand(request));
            return result.ToHttpResult();
        });

        app.MapPost("forgot-password/verify-mfa", async (ForgotPasswordVerifyMfaRequest request, ISender sender) =>
        {
            var result = await sender.Send(new VerifyForgotPasswordMfaCommand(request));
            return result.ToHttpResult();
        });

        app.MapPost("forgot-password/reset", async (ResetPasswordRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ResetPasswordCommand(request));
            return result.ToHttpResult();
        });
        #endregion
    }
}
