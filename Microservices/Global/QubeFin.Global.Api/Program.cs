using QubeFin.Core.Endpoint;
using QubeFin.Core.OpenApi;
using QubeFin.Core.Settings;
using QubeFin.Global.Application;
using QubeFin.Global.Infrastructure;
using QubeFin.Global.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQubeFinOpenApi("QubeFin Global API");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
//}

// app.UseHttpsRedirection();

app.UseQubeFinSwaggerUi("QubeFin Global API");
app.MapEndpoints();

app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin Global API";
});

app.Run();
