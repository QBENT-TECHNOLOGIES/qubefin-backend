using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace QubeFin.Tracking.Application
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails();
            problemDetails.Instance = httpContext.Request.Path;

            if (exception is ValidationException fluentException)
            {
                problemDetails.Title = "one or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                List<string> validationErrors = [];
                foreach (var error in fluentException.Errors)
                {
                    validationErrors.Add(error.ErrorMessage);
                }
                problemDetails.Extensions.Add("errors", validationErrors);
            }
            else if (exception is BadHttpRequestException badRequest)
            {
                // The body or query could not be read, e.g. a value that is not one of an enum's names ("kind": "Employee").
                // That is the caller's mistake: 400 with the reason, not 500.
                problemDetails.Title = "one or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                httpContext.Response.StatusCode = badRequest.StatusCode;
                var reason = exception.InnerException is System.Text.Json.JsonException json
                    ? $"Invalid value at {json.Path?.TrimStart('$', '.')}. Check the allowed values for that field."
                    : exception.Message;
                problemDetails.Extensions.Add("errors", new List<string> { reason });
            }
            else
            {
                problemDetails.Title = exception.Message;
            }

            logger.LogError($"{problemDetails.Title}");

            problemDetails.Status = httpContext.Response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
