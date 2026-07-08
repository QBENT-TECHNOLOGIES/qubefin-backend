using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Core.Security;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Persistence;

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
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        return services;
    }
}
