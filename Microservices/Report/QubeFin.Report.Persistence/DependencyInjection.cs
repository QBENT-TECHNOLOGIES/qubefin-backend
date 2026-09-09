using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using QubeFin.Report.Persistence.Repositories;

namespace QubeFin.Report.Persistence;

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
        services.AddScoped<IReportRepository, ReportRepository>();
        return services;
    }
}