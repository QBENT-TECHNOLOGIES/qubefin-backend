using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<QubeFinDataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DataConnection"), sqlOptions => sqlOptions.UseNetTopologySuite());
            options.EnableSensitiveDataLogging(false);
        });
        services.AddScoped<IUnitOfWork>(options => options.GetRequiredService<QubeFinDataContext>());
        services.AddHttpClient();

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITrackingScopeRepository, TrackingScopeRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IGeoTagRepository, GeoTagRepository>();
        services.AddScoped<IBcListRepository, BcListRepository>();
        services.AddScoped<IFileStorageRepository, FileStorageRepository>();
        return services;
    }
}
