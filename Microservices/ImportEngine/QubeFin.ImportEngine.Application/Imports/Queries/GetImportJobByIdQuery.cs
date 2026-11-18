using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Application.Imports.Services;

namespace QubeFin.ImportEngine.Application.Imports.Queries;

public record GetImportJobByIdQuery(Guid JobId) : IRequest<Result<ImportJobResult>>;

public class GetImportJobByIdQueryHandler(IImportJobService importJobService) : IRequestHandler<GetImportJobByIdQuery, Result<ImportJobResult>>
{
    public async Task<Result<ImportJobResult>> Handle(GetImportJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await importJobService.GetJobAsync(request.JobId, cancellationToken);
        if (job is null)
        {
            return Result.Fail<ImportJobResult>(new RecordNotFoundError("Import job not found."));
        }

        return Result.Ok(job);
    }
}
