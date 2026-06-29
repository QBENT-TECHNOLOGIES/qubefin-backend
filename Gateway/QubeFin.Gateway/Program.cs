using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using System.Net.Http.Headers;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "QubeFinCorsPolicy",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition", "X-File-Name");
        });
});

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 50;
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            var httpRequest = transformContext.HttpContext.Request;

            if (httpRequest.ContentType != null)
            {
                if (transformContext.ProxyRequest.Content == null)
                {
                    // Force YARP to create a stream content
                    transformContext.ProxyRequest.Content =
                        new StreamContent(httpRequest.Body);
                }

                transformContext.ProxyRequest.Content.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(httpRequest.ContentType);
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UsePathBase("/gateway");

app.MapReverseProxy();

app.UseCors("QubeFinCorsPolicy");

app.UseRateLimiter();

app.Use(async (context, next) =>
{
    var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (max != null)
    {
        max.MaxRequestBodySize = null; // unlimited
    }
    await next();
});




app.Run();