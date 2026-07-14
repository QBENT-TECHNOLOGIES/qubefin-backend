using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Core.Security;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Persistence;

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
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        return services;
    }
}