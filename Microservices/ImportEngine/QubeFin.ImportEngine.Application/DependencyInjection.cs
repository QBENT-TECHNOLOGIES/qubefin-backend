using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.ImportEngine.Application.Imports.Models;
using QubeFin.ImportEngine.Application.Imports.Services;
using System.Reflection;

namespace QubeFin.ImportEngine.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.Configure<ImportSettings>(configuration.GetSection("ImportSettings"));
        services.AddScoped<IExcelParsingService, ExcelParsingService>();
        services.AddScoped<IImportJobService, ImportJobService>();

        return services;
    }
}
