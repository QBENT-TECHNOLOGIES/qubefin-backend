using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace QubeFin.Core.OpenApi;

public static class SwaggerExtensions
{
    private const string BearerSchemeId = "Bearer";

    /// <summary>
    /// Registers the OpenAPI document (served at /openapi/v1.json) with service info
    /// and a JWT Bearer security scheme so Swagger UI shows an "Authorize" button.
    /// </summary>
    public static IServiceCollection AddQubeFinOpenApi(this IServiceCollection services, string title)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = title;
                document.Info.Version = "v1";

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[BearerSchemeId] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste the JWT access token (without the 'Bearer ' prefix)."
                };

                document.Security ??= [];
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerSchemeId, document)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    /// <summary>
    /// Serves Swagger UI at /swagger, reading the built-in OpenAPI document.
    /// </summary>
    public static WebApplication UseQubeFinSwaggerUi(this WebApplication app, string title)
    {
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", title);
            options.RoutePrefix = "swagger";
            options.DocumentTitle = title;
            options.EnablePersistAuthorization();
            options.DisplayRequestDuration();
        });

        return app;
    }
}
