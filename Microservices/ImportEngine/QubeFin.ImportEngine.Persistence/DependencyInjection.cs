using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.ImportEngine.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.ImportEngine.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<QubeFinDataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DataConnection"));
            options.EnableSensitiveDataLogging(false);
        });

        services.AddScoped<IImportTypeRepository, ImportTypeRepository>();
        services.AddScoped<IImportJobRepository, ImportJobRepository>();
        services.AddScoped<IImportDataRepository, ImportDataRepository>();
        services.AddScoped<IFileStorageRepository, FileStorageRepository>();

        return services;
    }
}
