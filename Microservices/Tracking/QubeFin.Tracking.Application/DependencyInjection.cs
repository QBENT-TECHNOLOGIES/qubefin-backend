using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QubeFin.Tracking.Application.Jobs;

namespace QubeFin.Tracking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        services.AddScoped<JobTaskPlanner>();
        services.AddScoped<JobNumberGenerator>();
        services.AddScoped<JobTaskCloser>();

        return services;
    }
}
