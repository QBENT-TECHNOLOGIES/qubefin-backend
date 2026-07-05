using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Security;
using QubeFin.Persistence;

namespace QubeFin.App.Persistence;

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

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        //services.AddScoped<IRoleRe>
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}