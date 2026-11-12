using QubeFin.Core.Endpoint;
using QubeFin.Core.OpenApi;
using QubeFin.Core.Settings;
using QubeFin.Tracking.Application;
using QubeFin.Tracking.Persistence;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddQubeFinOpenApi("QubeFin Tracking API");
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseQubeFinSwaggerUi("QubeFin Tracking API");
app.MapEndpoints();
app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin Tracking API";
});

app.Run();

