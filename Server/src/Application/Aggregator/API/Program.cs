using API.Endpoints;
using MassTransit;
using Modules.Students.Application;
using Modules.Students.Application.UseCases;
using Modules.Students.Infrastructure;
using Modules.Students.IntegrationEvents;
using Modules.Students.Persistence;
using Modules.Works.Application;
using Modules.Works.Application.UseCases;
using Modules.Works.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddStudentModulePersistence();
builder.AddStudentModuleInfrastructure();
builder.AddStudentModuleApplication();

builder.AddWorksModuleApplication();
builder.AddWorksModulePersistence();

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<GetStudentWorksRequestConsumer>();
	x.AddConsumer<WorkUploadedConsumer>();

	x.SetKebabCaseEndpointNameFormatter();
	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.ConfigureEndpoints(context);
	});
});

var app = builder.Build();

app.MapStudentEndpoints();
app.MapWorkEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
