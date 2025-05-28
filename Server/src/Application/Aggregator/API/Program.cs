using API.Endpoints;
using API.Extensions;
using API.Infrastructure;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Modules.Students.Application;
using Modules.Students.Infrastructure;
using Modules.Students.Persistence;
using Modules.Works.Application;
using Modules.Works.Infrastructure;
using Modules.Works.Persistence;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

builder.AddWorksModuleApplication();
builder.AddWorksModuleInfrastructure();
builder.AddWorksModulePersistence();

if (!builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddConfiguredMassTransit();
builder.Services.AddConfiguredCors(config);
builder.Services.AddConfiguredHangfire(config);

builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(config.GetConnectionString("Database")!)
    .AddRedis(config.GetConnectionString("Cache")!);

var app = builder.Build();

app.UseCors(config["CorsPolicy:Name"] ?? string.Empty);

app.MapStudentEndpoints();
app.MapWorkEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.UseExceptionHandler(opt => { });

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program;