using FluentResults;
using MediatR;
using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Persistence.Repositories;

namespace QubeFin.ImportEngine.Application.Imports.Queries;

public record GetActiveImportTypesQuery : IRequest<Result<List<ImportTypeSummary>>>;

public class GetActiveImportTypesQueryHandler(IImportTypeRepository importTypeRepository) : IRequestHandler<GetActiveImportTypesQuery, Result<List<ImportTypeSummary>>>
{
    public async Task<Result<List<ImportTypeSummary>>> Handle(GetActiveImportTypesQuery request, CancellationToken cancellationToken)
    {
        var importTypes = await importTypeRepository.GetActiveAsync(cancellationToken);

        var summaries = importTypes.Select(t => new ImportTypeSummary
        {
            Id = t.Id,
            Name = t.Name,
            TemplateUrl = t.TemplateUrl
        }).ToList();

        return Result.Ok(summaries);
    }
}
