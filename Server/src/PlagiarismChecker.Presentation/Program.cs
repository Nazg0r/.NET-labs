using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DataContext>(opt =>
{
	opt.UseNpgsql(connectionString);
}, ServiceLifetime.Singleton);

var app = builder.Build();

app.Run();
