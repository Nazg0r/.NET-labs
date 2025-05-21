using Microsoft.Extensions.Caching.Distributed;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using System.Text.Json;

namespace Modules.Works.Persistence.Repositories
{
	public class CachedWorkRepository(IWorkRepository repository, IDistributedCache cache)
		: IWorkRepository
	{
		public async Task<List<Work>> GetAllWorksAsync()
		{
			return await repository.GetAllWorksAsync();
		}

		public async Task<List<Work>> GetWorksByStudentIdAsync(string id)
		{
			return await repository.GetWorksByStudentIdAsync(id);
		}

		public async Task<Work?> GetWorkByIdAsync(Guid id)
		{
			var cachedWork = await cache.GetStringAsync(id.ToString());

			if (!string.IsNullOrEmpty(cachedWork))
				return JsonSerializer.Deserialize<Work>(cachedWork);

			var work = await repository.GetWorkByIdAsync(id);
			await cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(work));

			return work;
		}

		public async Task<Work> AddNewWorkAsync(Work work)
		{
			var storedWork = await repository.AddNewWorkAsync(work);
			await cache.SetStringAsync(storedWork.Id.ToString(), JsonSerializer.Serialize(storedWork));

			return storedWork;
		}

		public async Task<bool> DeleteWorkAsync(Guid id)
		{
			await cache.RemoveAsync(id.ToString());
			return await repository.DeleteWorkAsync(id);
		}
	}
}