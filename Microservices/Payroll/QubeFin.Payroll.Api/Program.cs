using QubeFin.Core.Endpoint;
using QubeFin.Core.Settings;
using QubeFin.Payroll.Application;
using QubeFin.Payroll.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
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

app.UseHttpsRedirection();
app.MapEndpoints();
app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Title = "QubeFin Payroll API";
});

app.Run();

