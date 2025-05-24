using BuildingBlocks.Contracts;
using BuildingBlocks.Models;
using Modules.Works.Domain.Exceptions;

namespace Modules.Works.Application.UseCases.BulkExport
{
	public class BulkExportHandler(
		IWorkRepository workRepository,
		ICsvBuilder csvBuilder
	)
		: IQueryHandler<Unit, byte[]>
	{
		public async Task<byte[]> HandleAsync(Unit query, CancellationToken cancellationToken)
		{
			var allWorks = await workRepository.GetAllWorksAsync();
			if (!allWorks.Any()) throw new StudentWorksNotFoundException();
			return csvBuilder.BuildWorksCsv(allWorks);
		}
	}
}