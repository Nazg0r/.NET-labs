using Microsoft.EntityFrameworkCore;
using Modules.Works.Domain.Constants;
using Modules.Works.Domain.Entities;

namespace Modules.Works.Persistence.Data
{
	public class WorkDbContext(DbContextOptions<WorkDbContext> options) : DbContext(options)
	{
		public DbSet<Work> Works { get; set; } = null!;
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasDefaultSchema(Module.Name.ToLower());
		}
	}
}
