using API.Endpoints;
using Modules.Students.Application;
using Modules.Students.Infrastructure;
using Modules.Students.Persistence;
using Modules.Works.Application;
using Modules.Works.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

builder.AddWorksModuleApplication();
builder.AddWorksModulePersistence();

var app = builder.Build();

app.MapStudentEndpoints();
app.MapWorkEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
