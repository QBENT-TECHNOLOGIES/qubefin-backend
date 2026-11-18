using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using QubeFin.Core.Results;
using QubeFin.ImportEngine.Application.Imports.Services;

namespace QubeFin.ImportEngine.Application.Imports.Commands;

public record CreateImportJobCommand(IFormFile File, Guid ImportTypeId, Guid UserId) : IRequest<Result<object>>;

public class CreateImportJobCommandHandler(IImportJobService importJobService) : IRequestHandler<CreateImportJobCommand, Result<object>>
{
    public async Task<Result<object>> Handle(CreateImportJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var jobId = await importJobService.CreateJobAsync(request.File, request.ImportTypeId, request.UserId, cancellationToken);
            var jobResult = await importJobService.ProcessJobAsync(jobId, cancellationToken);
            return Result.Ok<object>(jobResult);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<object>(new ValidationError(ex.Message));
        }
    }
}
