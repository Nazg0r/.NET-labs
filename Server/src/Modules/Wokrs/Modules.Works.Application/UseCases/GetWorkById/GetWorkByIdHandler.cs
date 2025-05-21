using BuildingBlocks.Contracts;
using Modules.Works.Application.Common.Mappings;
using Modules.Works.Domain.Exceptions;

namespace Modules.Works.Application.UseCases.GetWorkById
{
	public class GetWorkByIdHandler(IWorkRepository repository) : IQueryHandler<Guid, ProcessedWorkResponse>
	{
		public async Task<ProcessedWorkResponse> HandleAsync(Guid query, CancellationToken cancellationToken)
		{
			var work = await repository.GetWorkByIdAsync(query);
			if (work is null) throw new StudentWorkNotFoundException(query);

			return work.ToDto();
		}
	}
}