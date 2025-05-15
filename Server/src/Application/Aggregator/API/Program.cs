using API.Endpoints;
using API.Extensions;
using API.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Modules.Students.Application;
using Modules.Students.Infrastructure;
using Modules.Students.Persistence;
using Modules.Works.Application;
using Modules.Works.Persistence;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

builder.AddWorksModuleApplication();
builder.AddWorksModulePersistence();

builder.Services.AddConfiguredMassTransit();

builder.Services.AddConfiguredCors(config);

builder.Services.AddExceptionHandler<ErrorHandler>();

var app = builder.Build();

app.UseCors(config["CorsPolicy:Name"] ?? string.Empty);

app.MapStudentEndpoints();
app.MapWorkEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(opt => { });

app.Run();
