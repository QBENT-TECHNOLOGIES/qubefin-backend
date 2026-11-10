using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Collection.Persistence.Repositories;
using QubeFin.Core.Security;
using QubeFin.Persistence;

namespace QubeFin.Collection.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<QubeFinDataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DataConnection"));
            options.EnableSensitiveDataLogging(false);
        });
        services.AddScoped<IUnitOfWork>(options => options.GetRequiredService<QubeFinDataContext>());
        services.AddHttpClient();

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        return services;
    }
}
