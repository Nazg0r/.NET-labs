using Modules.Works.Application.Common.Models;
using Modules.Works.Application.UseCases.UploadWork;
using Modules.Works.Domain.Constants;
using Modules.Works.Infrastructure.Jobs.Shared;

namespace Modules.Works.Infrastructure.Jobs
{
	public class UploadFileJob(UploadWorkHandler handler)
	{
		public async Task ProcessAsync(string filePath, string studentId)
		{
			await using var stream = ReadFile.Read(filePath);

			var command = new UploadWorkCommand
			{
				FileStream = stream,
				StudentId = studentId,
				FileName = Path.GetFileName(filePath)[Length.GuidLength..]
			};

			await handler.HandleAsync(command, CancellationToken.None);

			File.Delete(filePath);
		}
	}
}
