using BuildingBlocks.Contracts;

namespace Modules.Works.Application.UseCases.BulkImport
{
	public class BulkImportHandler(
		ICsvParser parser,
		IBulkOperations bulkOperations) 
		: ICommandHandler<BulkImportCommand>
	{
		public async Task HandleAsync(BulkImportCommand command, CancellationToken cancellationToken)
		{
			await using var stream = command.FileStream;
			var works = parser.ParseCsv(stream);
			if (works == null || !works.Any())
				throw new InvalidOperationException("No works found in the provided CSV file.");
			await bulkOperations.InsertAsync(works, cancellationToken);
		}
	}
}
