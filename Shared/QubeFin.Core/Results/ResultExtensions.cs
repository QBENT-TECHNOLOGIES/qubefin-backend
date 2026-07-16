using FluentResults;
using Microsoft.AspNetCore.Http;

namespace QubeFin.Core.Results;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);
        }

        var error = result.Errors[0];

        return error switch
        {
            UnauthorizedError => Microsoft.AspNetCore.Http.Results.Unauthorized(),
            ForbiddenError => Microsoft.AspNetCore.Http.Results.Forbid(),
            RecordNotFoundError => Microsoft.AspNetCore.Http.Results.NotFound(error),
            ValidationError => Microsoft.AspNetCore.Http.Results.BadRequest(error),
            _ => Microsoft.AspNetCore.Http.Results.Problem(error.Message)
        };
    }
}
