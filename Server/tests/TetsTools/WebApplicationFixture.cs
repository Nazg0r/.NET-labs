using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;
using Testcontainers.RabbitMq;
using Modules.Students.Persistence.Data;
using Modules.Works.Persistence.Data;
using MassTransit;
using Modules.Students.Application.UseCases;
using Modules.Works.Application.UseCases;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace TestsTools
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

		private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
			.WithImage("rabbitmq:3-management")
			.WithUsername("guest")
			.WithPassword("guest")
			.Build();

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");

			builder.ConfigureAppConfiguration((context, config) =>
			{
				var testConfig = new Dictionary<string, string>
				{
					["JWT:Secret"] = "gH2e3nE7A1Jr87Kv5G2JXyMPC1+jf0BZoD+zPrw3JwY=",
					["JWT:Expires"] = "60",
					["UploadDir:Path"] = ".//..//Uploads"
				};

				config.AddInMemoryCollection(testConfig!);
			});

			builder.ConfigureTestServices(services =>
			{
				var StudentDBDescriptor = services
					.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<StudentDbContext>));
				var WorkDBDescriptor = services
					.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<WorkDbContext>));
				var cacheDescriptor = services
					.SingleOrDefault(s => s.ServiceType == typeof(RedisCacheOptions));

				if (StudentDBDescriptor is not null)
					services.Remove(StudentDBDescriptor);
				if (WorkDBDescriptor is not null)
					services.Remove(WorkDBDescriptor);
				if (cacheDescriptor is not null)
					services.Remove(cacheDescriptor);

				services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));
				services.RemoveAll(typeof(IConfigureOptions<JwtBearerOptions>));
				services.RemoveAll(typeof(IConfigureOptions<MassTransitHostOptions>));

				services.AddDbContext<StudentDbContext>(opt => { opt.UseNpgsql(_dbContainer.GetConnectionString()); });

				services.AddDbContext<WorkDbContext>(opt =>
				{
					opt.UseNpgsql(_dbContainer.GetConnectionString())
						.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
					;
				});

				services.AddStackExchangeRedisCache(opt => { opt.Configuration = _redisCache.GetConnectionString(); });

				services.AddMassTransit(x =>
				{
					x.AddConsumer<GetStudentWorksRequestConsumer>();
					x.AddConsumer<WorkUploadedConsumer>();

					x.SetKebabCaseEndpointNameFormatter();
					x.UsingRabbitMq((context, cfg) =>
					{
						cfg.Host(_rabbitMqContainer.Hostname, "/", h =>
						{
							h.Username("guest");
							h.Password("guest");
						});
						cfg.ConfigureEndpoints(context);
					});
				});

				services.AddAuthentication("Test")
					.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
						"Test", options => { });
			});
		}

		public async Task InitializeAsync()
		{
			await _dbContainer.StartAsync();
			await _redisCache.StartAsync();
			await _rabbitMqContainer.StartAsync();

			using var scope = Services.CreateScope();
			var studentDbContext = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
			var workDbContext = scope.ServiceProvider.GetRequiredService<WorkDbContext>();

			await studentDbContext.Database.MigrateAsync();
			await workDbContext.Database.MigrateAsync();
		}

		async Task IAsyncLifetime.DisposeAsync()
		{
			using var scope = Services.CreateScope();
			var studentDbContext = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
			var workDbContext = scope.ServiceProvider.GetRequiredService<WorkDbContext>();

			await studentDbContext.Database.EnsureDeletedAsync();
			await workDbContext.Database.EnsureDeletedAsync();

			await _dbContainer.StopAsync();
			await _redisCache.StopAsync();
			await _rabbitMqContainer.StopAsync();
		}
	}
}