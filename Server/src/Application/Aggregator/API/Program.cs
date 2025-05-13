using API.Endpoints;
using Modules.Students.Application;
using Modules.Students.Infrastructure;
using Modules.Students.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

var app = builder.Build();

app.MapStudentEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
