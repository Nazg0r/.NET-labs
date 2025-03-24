namespace DataAccess.Data
{
	public class DataContext(DbContextOptions opt) : DbContext(opt)
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
		}
	}
}
