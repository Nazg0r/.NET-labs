using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Modules.Students.Persistence.Data
{
	public class StudentDbContextFactory : IDesignTimeDbContextFactory<StudentDbContext>
	{
		public StudentDbContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<StudentDbContext>();
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));

			return new StudentDbContext(optionsBuilder.Options);
		}
	}
}