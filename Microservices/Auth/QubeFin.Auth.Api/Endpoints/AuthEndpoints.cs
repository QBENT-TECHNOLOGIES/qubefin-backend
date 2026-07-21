using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Auth.Application.Accounts.Commands;
using QubeFin.Auth.Application.Accounts.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

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

        app.MapPost("refresh-token", async ([FromHeader(Name = "Refresh-Token")] string refreshToken, ISender sender) =>
        {
            var result = await sender.Send(new ValidateRefreshTokenQuery(refreshToken));
            return result.ToHttpResult();
        });
    }
}
