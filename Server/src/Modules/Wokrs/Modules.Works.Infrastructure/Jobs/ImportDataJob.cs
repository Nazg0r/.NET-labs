using Modules.Works.Application.Common.Models;
using Modules.Works.Application.UseCases.BulkImport;
using Modules.Works.Domain.Constants;
using Modules.Works.Infrastructure.Jobs.Shared;

namespace Modules.Works.Infrastructure.Jobs
{
    public class ImportDataJob(BulkImportHandler handler)
    {
        public async Task ProcessAsync(string filePath)
        {
            await using var stream = ReadFile.Read(filePath);

            var command = new BulkImportCommand
            {
                FileStream = stream,
                FileName = filePath[Length.GuidLength..]
            };

            await handler.HandleAsync(command, CancellationToken.None);

            File.Delete(filePath);
        }
    }
}