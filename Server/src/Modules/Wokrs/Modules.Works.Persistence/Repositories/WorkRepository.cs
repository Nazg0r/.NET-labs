using Microsoft.EntityFrameworkCore;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using Modules.Works.Persistence.Data;

namespace Modules.Works.Persistence.Repositories
{
	public class WorkRepository(WorkDbContext context) : IWorkRepository
	{
		public async Task<Work?> GetWorkByIdAsync(Guid id) =>
			await context.Works.FindAsync(id);

		public async Task<List<Work>> GetAllWorksAsync() =>
			await context.Works.AsNoTracking().ToListAsync();

		public async Task<List<Work>> GetWorksByStudentIdAsync(string id) =>
			await context.Works.AsNoTracking().Where(w => w.StudentId == id).ToListAsync();

		public async Task<Work> AddNewWorkAsync(Work work)
		{
			await context.Works.AddAsync(work);
			await context.SaveChangesAsync();

			return work;
		}

		public async Task<bool> DeleteWorkAsync(Guid id)
		{
			var affectedRows = await context.Works
				.Where(w => w.Id == id)
				.ExecuteDeleteAsync();

			return affectedRows > 0;
		}
	}
}