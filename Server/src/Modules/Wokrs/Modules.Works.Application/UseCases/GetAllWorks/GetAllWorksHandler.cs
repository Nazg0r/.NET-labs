using BuildingBlocks.Contracts;
using Modules.Works.Application.Common.Mappings;
using Modules.Works.Domain.Exceptions;

namespace Modules.Works.Application.UseCases.GetAllWorks
{
	public class GetAllWorksHandler(IWorkRepository repository) : IQueryHandler<Unit, List<ProcessedWorkResponse>>
	{
		public async Task<List<ProcessedWorkResponse>> HandleAsync(Unit query, CancellationToken cancellationToken)
		{
			var works = await repository.GetAllWorksAsync();
			if (!works.Any()) throw new StudentWorksNotFoundException();

			return works.Select(w => w.ToDto()).ToList();
		}
	}
}