using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DataContext>(opt =>
{
	opt.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);

builder.Services.AddRepositories();
builder.Services.AddCustomAuthentification(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();

app.Run();
