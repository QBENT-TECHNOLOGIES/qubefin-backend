using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.ImportEngine.Persistence.Models;
using QubeFin.Persistence;

namespace QubeFin.ImportEngine.Persistence.Repositories;

public interface IImportTypeRepository
{
    Task<List<ImportTypeRow>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<ImportTypeRow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class ImportTypeRepository(QubeFinDataContext context) : IImportTypeRepository
{
    public async Task<List<ImportTypeRow>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.SqlQuery<ImportTypeRow>(
            $"SELECT Id, Name, SpName, TemplateUrl, Columns, IsActive FROM [Global].[Tbl_ImportType] WHERE IsActive = 1")
            .ToListAsync(cancellationToken);
    }

    public async Task<ImportTypeRow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rows = await context.Database.SqlQuery<ImportTypeRow>(
            $"SELECT Id, Name, SpName, TemplateUrl, Columns, IsActive FROM [Global].[Tbl_ImportType] WHERE Id = {id}")
            .ToListAsync(cancellationToken);
        return rows.SingleOrDefault();
    }
}
