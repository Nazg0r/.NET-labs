using BuildingBlocks.Contracts;
using BuildingBlocks.Models;

namespace Modules.Works.Application.UseCases.BulkExport
{
	public class BulkExportHandler(
		IBulkOperations bulkOperations)
		: IQueryHandler<Unit, byte[]>
	{
		public async Task<byte[]> HandleAsync(Unit query, CancellationToken cancellationToken) =>
			await bulkOperations.ExportAsync(cancellationToken);
	}
}