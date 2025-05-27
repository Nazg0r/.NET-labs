using Hangfire;
using Hangfire.PostgreSql;

namespace API.Extensions
{
	public static class ConfigureHangfire
	{
		public static void AddConfiguredHangfire(this IServiceCollection services, IConfiguration config)
		{
			services.AddHangfire(hConf =>
			{
				hConf.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UsePostgreSqlStorage(options
						=> options.UseNpgsqlConnection(config.GetConnectionString("Database")));
			});

			services.AddHangfireServer();
		}
	}
}