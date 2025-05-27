using BuildingBlocks.Contracts;
using Modules.Works.Application.Common.Mappings;
using Modules.Works.Domain.Exceptions;

namespace Modules.Works.Application.UseCases.GetUploadedWork
{
	public class GetUploadedWorkHandler(
		IJobRepository jobRepository,
		IWorkRepository workRepository)
		: IQueryHandler<string, ProcessedWorkResponse>
	{
		public async Task<ProcessedWorkResponse> HandleAsync(string query, CancellationToken cancellationToken)
		{
			var jobResult = await jobRepository.GetAndRemoveJobResultByJobIdAsync(query);
			if (jobResult is null) throw new JobResultNotFoundException(query);

			var work = await workRepository.GetWorkByIdAsync(jobResult.WorkId);
			if (work is null) throw new StudentWorkNotFoundException(jobResult.WorkId);

			return work.ToDto();
		}
	}
}