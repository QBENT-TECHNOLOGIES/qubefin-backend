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
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    //.AddTransforms(builderContext =>
    //{
    //    builderContext.AddRequestTransform(async transformContext =>
    //    {
    //        var httpRequest = transformContext.HttpContext.Request;

    //        if (httpRequest.ContentType != null)
    //        {
    //            if (transformContext.ProxyRequest.Content == null)
    //            {
    //                // Force YARP to create a stream content
    //                transformContext.ProxyRequest.Content =
    //                    new StreamContent(httpRequest.Body);
    //            }

    //            transformContext.ProxyRequest.Content.Headers.ContentType =
    //                MediaTypeHeaderValue.Parse(httpRequest.ContentType);
    //        }
    //    });
    //});

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

// app.UsePathBase("/gateway");

app.UseCors("QubeFinCorsPolicy");

app.UseRateLimiter();

app.Use(async (context, next) =>
{
    var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (max != null)
    {
        max.MaxRequestBodySize = null; // unlimited
    }

    context.Request.EnableBuffering();

    using var reader = new StreamReader(
        context.Request.Body,
        leaveOpen: true);

    var body = await reader.ReadToEndAsync();

    Console.WriteLine($"ContentType: {context.Request.ContentType}");
    Console.WriteLine($"ContentLength: {context.Request.ContentLength}");
    Console.WriteLine($"Body: {body}");

    context.Request.Body.Position = 0;

    await next();
});

app.MapReverseProxy();

app.Run();