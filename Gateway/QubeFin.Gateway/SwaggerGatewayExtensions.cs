using System.Text;
using System.Text.Json.Nodes;
using Yarp.ReverseProxy.Transforms;

namespace QubeFin.Gateway;

public static class SwaggerGatewayExtensions
{
    private const string OpenApiDocSuffix = "/openapi/v1.json";

    // Gateway route prefix -> display name. Prefixes match the gateway routes used by the frontend (ApiPaths).
    private static readonly (string Prefix, string Name)[] Services =
    [
        ("auth-api", "QubeFin Auth API"),
        ("app-api", "QubeFin App API"),
        ("global-api", "QubeFin Global API"),
        ("hrms-api", "QubeFin Hrms API"),
        ("payroll-api", "QubeFin Payroll API"),
        ("report-api", "QubeFin Report API"),
        ("tracking-api", "QubeFin Tracking API"),
    ];

    /// <summary>
    /// When a service's OpenAPI document is fetched through the gateway (e.g. /app-api/openapi/v1.json),
    /// replace its "servers" with the gateway prefix (e.g. /app-api) so "Try it out" calls go via the gateway.
    /// </summary>
    public static IReverseProxyBuilder AddOpenApiServerRewrite(this IReverseProxyBuilder builder)
    {
        return builder.AddTransforms(builderContext =>
        {
            builderContext.AddResponseTransform(async transformContext =>
            {
                var request = transformContext.HttpContext.Request;
                var path = request.Path.Value;

                if (path is null
                    || !path.EndsWith(OpenApiDocSuffix, StringComparison.OrdinalIgnoreCase)
                    || transformContext.ProxyResponse is not { IsSuccessStatusCode: true } proxyResponse)
                {
                    return;
                }

                var document = JsonNode.Parse(await proxyResponse.Content.ReadAsStreamAsync());
                if (document is not JsonObject root)
                {
                    return;
                }

                var prefix = request.PathBase + path[..^OpenApiDocSuffix.Length];
                root["servers"] = new JsonArray(new JsonObject { ["url"] = prefix.ToString() });

                var bytes = Encoding.UTF8.GetBytes(root.ToJsonString());
                var response = transformContext.HttpContext.Response;
                transformContext.SuppressResponseBody = true;
                response.Headers.Remove("Content-Encoding");
                response.ContentLength = bytes.Length;
                response.ContentType = "application/json; charset=utf-8";
                await response.Body.WriteAsync(bytes);
            });
        });
    }

    /// <summary>
    /// Serves a combined Swagger UI at /swagger with a definition dropdown for every microservice.
    /// </summary>
    public static WebApplication UseGatewaySwaggerUi(this WebApplication app)
    {
        app.UseSwaggerUI(options =>
        {
            foreach (var (prefix, name) in Services)
            {
                options.SwaggerEndpoint($"/{prefix}{OpenApiDocSuffix}", name);
            }

            options.RoutePrefix = "swagger";
            options.DocumentTitle = "QubeFin API Gateway";
            options.EnablePersistAuthorization();
            options.DisplayRequestDuration();
        });

        return app;
    }
}
