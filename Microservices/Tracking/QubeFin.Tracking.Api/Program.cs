using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Settings;
using QubeFin.Tracking.Application;
using QubeFin.Tracking.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddMemoryCache();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapEndpoints();
app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin Tracking API";
});

app.Run();
