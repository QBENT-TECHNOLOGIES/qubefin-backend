using QubeFin.Core.Endpoint;
using QubeFin.Core.Settings;
using QubeFin.ImportEngine.Application;
using QubeFin.ImportEngine.Infrastructure;
using QubeFin.ImportEngine.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.MapOpenApi();

app.MapEndpoints();

app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin ImportEngine API";
});

app.Run();
