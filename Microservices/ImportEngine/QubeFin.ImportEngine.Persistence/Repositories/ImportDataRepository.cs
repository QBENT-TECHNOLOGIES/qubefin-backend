using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.ImportEngine.Persistence.Repositories;

public interface IImportDataRepository
{
    Task ExecuteImportSpAsync(string spName, string dataJson, CancellationToken cancellationToken = default);
}

public class ImportDataRepository(QubeFinDataContext context) : IImportDataRepository
{
    public async Task ExecuteImportSpAsync(string spName, string dataJson, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        await context.Database.ExecuteSqlRawAsync(
            $"EXEC {spName} @Data",
            [new SqlParameter("@Data", dataJson)], cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
