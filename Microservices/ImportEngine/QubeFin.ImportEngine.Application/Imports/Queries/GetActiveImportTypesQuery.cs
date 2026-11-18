using FluentResults;
using MediatR;
using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Persistence.Repositories;

namespace QubeFin.ImportEngine.Application.Imports.Queries;

public record GetActiveImportTypesQuery : IRequest<Result<List<ImportTypeSummary>>>;

public class GetActiveImportTypesQueryHandler(IImportTypeRepository importTypeRepository, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetActiveImportTypesQuery, Result<List<ImportTypeSummary>>>
{
    public async Task<Result<List<ImportTypeSummary>>> Handle(GetActiveImportTypesQuery request, CancellationToken cancellationToken)
    {
        var importTypes = await importTypeRepository.GetActiveAsync(cancellationToken);

        var summaries = await Task.WhenAll(importTypes.Select(async t => new ImportTypeSummary
        {
            Id = t.Id,
            Name = t.Name,
            TemplateUrl = await GetFileUrl(t.TemplateUrl, cancellationToken)
        }));

        return Result.Ok(summaries.ToList());
    }

    public async Task<string> GetFileUrl(string? key, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(key) ?  await fileStorageRepository.GetFileUrlAsync("ImportEngine/" + key, cancellationToken) : key;
    }
}
