using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Works.Application.Contracts;
using Modules.Works.Persistence.Data;
using Modules.Works.Persistence.Repositories;

namespace Modules.Works.Persistence
{
	public static class DependencyInjection
	{
		public static void AddWorksModulePersistence(this IHostApplicationBuilder builder)
		{
			var connectionString = builder.Configuration.GetConnectionString("Database");
			var connectionStringCache = builder.Configuration.GetConnectionString("Cache")!;

			builder.Services.AddDbContext<WorkDbContext>(
				opt => opt.UseNpgsql(connectionString), ServiceLifetime.Singleton);

			builder.Services.AddSingleton<IWorkRepository, WorkRepository>();
			builder.Services.Decorate<IWorkRepository, CachedWorkRepository>();

			builder.Services.AddStackExchangeRedisCache(opt =>
			{
				opt.Configuration = connectionStringCache;
			});
		}
	}
}