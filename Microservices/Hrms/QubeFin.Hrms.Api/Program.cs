using QubeFin.Core.Endpoint;
using QubeFin.Core.OpenApi;
using QubeFin.Core.Settings;
using QubeFin.Hrms.Application;
using QubeFin.Hrms.Infrastructure;
using QubeFin.Hrms.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQubeFinOpenApi("QubeFin Hrms API");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

app.UseQubeFinSwaggerUi("QubeFin Hrms API");
app.MapEndpoints();

app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin Hrms API";
});

app.Run();
