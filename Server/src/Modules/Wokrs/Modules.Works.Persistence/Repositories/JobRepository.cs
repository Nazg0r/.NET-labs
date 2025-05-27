using Microsoft.EntityFrameworkCore;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using Modules.Works.Persistence.Data;

namespace Modules.Works.Persistence.Repositories
{
	public class JobRepository(WorkDbContext context) : IJobRepository
	{
		public async Task<bool> StoreJobResultAsync(UploadFIleJobResult jobResult)
		{
			await context.UploadFIleJobResults.AddAsync(jobResult);
			var affectedRows = await context.SaveChangesAsync();

			return affectedRows > 0;
		}

		public async Task<UploadFIleJobResult?> GetAndRemoveJobResultByJobIdAsync(string jobId) {

			var result = await context.UploadFIleJobResults
				.FirstOrDefaultAsync(r => r.JobId == jobId);

			await context.UploadFIleJobResults
				.Where(r => r.JobId == jobId)
				.ExecuteDeleteAsync();

			return result;
		}
	}
}