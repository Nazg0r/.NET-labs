using MassTransit.Internals;
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
			(List<Work>)await GetWorksByStudentIdAsyncCompiled(context, id).ToListAsync();

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

		private static readonly Func<WorkDbContext, string, IAsyncEnumerable<Work>> GetWorksByStudentIdAsyncCompiled =
			EF.CompileAsyncQuery((WorkDbContext context, string studentId) =>
				context.Works.AsNoTracking().Where(w => w.StudentId == studentId));
	}
}