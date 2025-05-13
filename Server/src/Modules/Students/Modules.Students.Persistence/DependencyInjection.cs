using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Students.Infrastructure.Identity;
using Modules.Students.Persistence.Data;

namespace Modules.Students.Persistence
{
	public static class DependencyInjection
	{
		public static void AddStudentModulePersistence(this IHostApplicationBuilder builder)
		{
			var connectionString = builder.Configuration.GetConnectionString("Database");

			builder.Services.AddDbContext<StudentDbContext>(
				opt => { opt.UseNpgsql(connectionString); }, ServiceLifetime.Singleton);

			builder.Services.AddIdentity<StudentIdentity, IdentityRole>(opt =>
				{
					opt.Password.RequiredLength = 8;
					opt.Password.RequireDigit = false;
					opt.Password.RequireLowercase = true;
					opt.Password.RequireUppercase = false;
					opt.Password.RequiredUniqueChars = 4;
					opt.Password.RequireNonAlphanumeric = false;
				})
				.AddEntityFrameworkStores<StudentDbContext>()
				.AddDefaultTokenProviders();
		}
	}
}