using DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DataAccess.Repositories
{
	public class CachedStudentWorkRepository(
		IStudentWorkRepository repository,
		IDistributedCache cache)
		: IStudentWorkRepository
	{
		public async Task<List<StudentWork>> GetAllWorksAsync()
		{
			return await repository.GetAllWorksAsync();
		}

		public async Task<StudentWork?> GetWorkByIdAsync(Guid id)
		{
			var cachedWork = await cache.GetStringAsync(id.ToString());

			if (!string.IsNullOrEmpty(cachedWork))
				return JsonSerializer.Deserialize<StudentWork>(cachedWork);

			var work = await repository.GetWorkByIdAsync(id);
			await cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(work));

			return work;
		}

		public async Task<StudentWork> AddNewWorkAsync(StudentWork work)
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
