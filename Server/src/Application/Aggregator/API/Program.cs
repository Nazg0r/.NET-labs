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
var env = builder.Environment;

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

builder.AddWorksModuleApplication();
builder.AddWorksModuleInfrastructure();
builder.AddWorksModulePersistence();

builder.Services.AddConfiguredHangfire(config, env);

if (!env.IsEnvironment("Testing"))
{
    builder.Services.AddConfiguredCors(config);
    builder.Services.AddConfiguredMassTransit();
    builder.Services.AddHealthChecks()
        .AddNpgSql(config.GetConnectionString("Database")!)
        .AddRedis(config.GetConnectionString("Cache")!);
}

builder.Services.AddExceptionHandler<ErrorHandler>();
var app = builder.Build();

if (!env.IsEnvironment("Testing"))
    app.UseCors(config["CorsPolicy:Name"] ?? string.Empty);

app.MapStudentEndpoints();
app.MapWorkEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.UseExceptionHandler(opt => { });

if (!env.IsEnvironment("Testing"))
    app.UseHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

app.Run();

public partial class Program;