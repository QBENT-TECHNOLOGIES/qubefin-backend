using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.ImportEngine.Persistence.Models;
using QubeFin.Persistence;

namespace QubeFin.ImportEngine.Persistence.Repositories;

public interface IImportJobRepository
{
    Task<Guid> CreateAsync(Guid importTypeId, string originalFileName, string storedFilePath, Guid createdBy, CancellationToken cancellationToken = default);
    Task<ImportJobRow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetProcessingAsync(Guid id, CancellationToken cancellationToken = default);
    Task CompleteAsync(Guid id, string status, int totalRows, int successRows, int failedRows, string? errorDetails, CancellationToken cancellationToken = default);
}

public class ImportJobRepository(QubeFinDataContext context) : IImportJobRepository
{
    public async Task<Guid> CreateAsync(Guid importTypeId, string originalFileName, string storedFilePath, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var parameters = new[]
        {
            new SqlParameter("@p_Id", id),
            new SqlParameter("@p_ImportTypeId", importTypeId),
            new SqlParameter("@p_OriginalFileName", originalFileName),
            new SqlParameter("@p_StoredFilePath", storedFilePath),
            new SqlParameter("@p_CreatedBy", createdBy),
        };

        await context.Database.ExecuteSqlRawAsync(@"INSERT INTO [Global].[Tbl_ImportJob]
            (Id, ImportTypeId, OriginalFileName, StoredFilePath, Status, CreatedBy, CreatedAt)
            VALUES (@p_Id, @p_ImportTypeId, @p_OriginalFileName, @p_StoredFilePath, 'Pending', @p_CreatedBy, SYSUTCDATETIME())",
            parameters, cancellationToken);

        return id;
    }

    public async Task<ImportJobRow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rows = await context.Database.SqlQuery<ImportJobRow>(
            $"SELECT Id, ImportTypeId, OriginalFileName, StoredFilePath, Status, TotalRows, SuccessRows, FailedRows, ErrorDetails, CreatedBy, CreatedAt, CompletedAt FROM [Global].[Tbl_ImportJob] WHERE Id = {id}")
            .ToListAsync(cancellationToken);
        return rows.SingleOrDefault();
    }

    public async Task SetProcessingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await context.Database.ExecuteSqlRawAsync(
            "UPDATE [Global].[Tbl_ImportJob] SET Status = 'Processing' WHERE Id = @p_Id",
            [new SqlParameter("@p_Id", id)], cancellationToken);
    }

    public async Task CompleteAsync(Guid id, string status, int totalRows, int successRows, int failedRows, string? errorDetails, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@p_Id", id),
            new SqlParameter("@p_Status", status),
            new SqlParameter("@p_TotalRows", totalRows),
            new SqlParameter("@p_SuccessRows", successRows),
            new SqlParameter("@p_FailedRows", failedRows),
            new SqlParameter("@p_ErrorDetails", (object?)errorDetails ?? DBNull.Value),
        };

        await context.Database.ExecuteSqlRawAsync(@"UPDATE [Global].[Tbl_ImportJob]
            SET Status = @p_Status, TotalRows = @p_TotalRows, SuccessRows = @p_SuccessRows,
                FailedRows = @p_FailedRows, ErrorDetails = @p_ErrorDetails, CompletedAt = SYSUTCDATETIME()
            WHERE Id = @p_Id", parameters, cancellationToken);
    }
}
