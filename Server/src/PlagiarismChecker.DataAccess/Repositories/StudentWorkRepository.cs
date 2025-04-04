using DataAccess.Data;
using DataAccess.Interfaces;

namespace DataAccess.Repositories
{
	public class StudentWorkRepository(DataContext context)
		: IStudentWorkRepository
	{
		public async Task<StudentWork?> GetWorkByIdAsync(Guid id) =>
			await context.StudentWorks.FindAsync(id);

		public async Task<List<StudentWork>> GetAllWorksAsync() =>
			await context.StudentWorks.ToListAsync();

		public async Task<StudentWork> AddNewWorkAsync(StudentWork work)
		{
			await context.StudentWorks.AddAsync(work);
			await context.SaveChangesAsync();

			return work;
		}

		public async Task<bool> DeleteWorkAsync(Guid id)
		{
			var studentWork = await context.StudentWorks.FindAsync(id);

			if (studentWork is null)  return false;

			context.Remove(studentWork);
			await context.SaveChangesAsync();

			return true;
		}
	}
}
