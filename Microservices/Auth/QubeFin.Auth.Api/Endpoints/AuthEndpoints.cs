using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Auh.Application.Accounts.Commands;
using QubeFin.Auth.Application.Accounts.Commands;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

namespace QubeFin.Auth.Api.Endpoints;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("validate-login", async (HttpContext httpContext, ValidtateLoginCommand request, [FromHeader(Name = "Device-Id")] string? deviceId, ISender sender, IPublisher publisher) =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            request = request with
            {
                DeviceId = deviceId,
                UserAgent = userAgent
            };
            var result = await sender.Send(request);
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
        });

        app.MapPost("verify-mfa", async (VerifyMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
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
        });
    }
}
