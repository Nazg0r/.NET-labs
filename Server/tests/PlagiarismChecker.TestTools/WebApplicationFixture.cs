using DataAccess.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace PlagiarismChecker
{
	public class WebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
	{
		private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
			.WithImage("postgres")
			.WithDatabase("plagiarism_checker_test")
			.WithUsername("postgres")
			.WithPassword("postgres")
			.Build();

		private readonly RedisContainer _redisCache = new RedisBuilder()
			  .WithImage("redis:7.0")
			  .Build();

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestServices(services =>
			{
				var DBDescriptor = services
					.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<DataContext>));
				var cacheDescriptor = services
					.SingleOrDefault(s => s.ServiceType == typeof(RedisCacheOptions));

				if (DBDescriptor is not null)
					services.Remove(DBDescriptor);
				if (cacheDescriptor is not null)
					services.Remove(cacheDescriptor);

				services.AddDbContext<DataContext>(opt =>
				{
					opt.UseLazyLoadingProxies()
						.UseNpgsql(_dbContainer.GetConnectionString());
				}, ServiceLifetime.Singleton);

				services.AddStackExchangeRedisCache(opt =>
				{
					opt.Configuration = _redisCache.GetConnectionString();
				});

				services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));
				services.RemoveAll(typeof(IConfigureOptions<JwtBearerOptions>));

				services.AddAuthentication("Test")
					.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
						"Test", options => { });
			});
		}

		public async Task InitializeAsync()
		{
			await _dbContainer.StartAsync();
			await _redisCache.StartAsync();

			using var scope = Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
			await dbContext.Database.MigrateAsync();
		}

		async Task IAsyncLifetime.DisposeAsync()
		{
			using var scope = Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
			await dbContext.Database.EnsureDeletedAsync();

			await _dbContainer.StopAsync();
			await _redisCache.StopAsync();
		}
	}
}
