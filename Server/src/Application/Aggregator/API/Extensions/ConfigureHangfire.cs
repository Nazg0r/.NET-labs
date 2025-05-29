using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;

namespace API.Extensions
{
    public static class ConfigureHangfire
    {
        public static void AddConfiguredHangfire(this IServiceCollection services, IConfiguration config,
            IWebHostEnvironment env)
        {
            services.AddHangfire(hConf =>
            {
                hConf.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                if (!env.IsEnvironment("Testing"))
                    hConf.UsePostgreSqlStorage(options
                        => options.UseNpgsqlConnection(config.GetConnectionString("Database")));
                else
                    hConf.UseMemoryStorage();

            });

            services.AddHangfireServer();
        }
    }
}