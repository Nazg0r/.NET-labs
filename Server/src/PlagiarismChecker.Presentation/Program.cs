using DataAccess.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DataContext>(opt =>
{
	opt.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddCustomAuthentification(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();

app.Run();
