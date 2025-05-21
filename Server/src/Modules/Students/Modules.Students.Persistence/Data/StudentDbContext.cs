using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Students.Domain.Constants;
using Modules.Students.Domain.Entities;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Persistence.Data
{
	public class StudentDbContext(DbContextOptions<StudentDbContext> opt) 
		: IdentityDbContext<StudentIdentity>(opt)
	{
		public DbSet<Student> Students { get; set; } = default!;
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasDefaultSchema(Module.Name.ToLower());
		}
	}
}