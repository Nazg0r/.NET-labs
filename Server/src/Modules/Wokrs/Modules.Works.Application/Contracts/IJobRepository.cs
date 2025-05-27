namespace Modules.Works.Application.Contracts
{
	public interface IJobRepository
	{
		public Task<bool> StoreJobResultAsync(UploadFIleJobResult jobResult);
		public Task<UploadFIleJobResult?> GetAndRemoveJobResultByJobIdAsync(string jobId);
	}
}
