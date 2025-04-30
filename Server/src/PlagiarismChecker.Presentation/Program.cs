using DataAccess.Data;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;
using Shared.Errors.Handler;

var builder = WebApplication.CreateBuilder(args);
var connectionStringDb = builder.Configuration.GetConnectionString("Database")!;
var connectionStringCache = builder.Configuration.GetConnectionString("Cache")!;

builder.Services.AddDbContext<DataContext>(opt =>
{
	opt.UseLazyLoadingProxies()
	.UseNpgsql(connectionStringDb);
}, ServiceLifetime.Singleton);

builder.Services.AddStackExchangeRedisCache(opt =>
{
	opt.Configuration = connectionStringCache;
});

builder.Services.AddCors(opt =>
{
	opt.AddPolicy("AllowFront", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			.AllowCredentials()
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddCustomAuthentification(builder.Configuration);
builder.Services.AddExceptionHandler<CustomErrorHandler>();
builder.Services.AddHealthChecks()
	.AddNpgSql(connectionStringDb)
	.AddRedis(connectionStringCache);

var app = builder.Build();

app.UseCors("AllowFront");

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(opt => { });

app.UseHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
