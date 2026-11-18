using Microsoft.AspNetCore.Http;
using QubeFin.ImportEngine.Application.Imports.Models;

namespace QubeFin.ImportEngine.Application.Imports.Services;

public interface IImportJobService
{
    Task<Guid> CreateJobAsync(IFormFile file, Guid importTypeId, Guid userId, CancellationToken cancellationToken = default);

    Task<ImportJobResult> ProcessJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    Task<ImportJobResult?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}
