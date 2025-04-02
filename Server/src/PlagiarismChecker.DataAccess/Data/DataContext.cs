using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace DataAccess.Data
{
	public class DataContext(DbContextOptions<DataContext> opt)
		: IdentityDbContext<Student>(opt)
	{
		public DbSet<Student> Students { get; set; }
		public DbSet<StudentWork> StudentWorks{ get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Student>()
				.HasMany(w => w.Works)
				.WithOne(s => s.Student)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<StudentWork>()
				.HasOne(s => s.Student)
				.WithMany(w => w.Works)
				.HasForeignKey(w => w.StudentId);
		}
	}
}
