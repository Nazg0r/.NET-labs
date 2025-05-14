using Modules.Works.Application.Common.Mappings;

namespace Modules.Works.Application.UseCases.UploadWork
{
	public class UploadWorkHandler(IWorkRepository repository) : ICommandHandler<UploadWorkCommand, ProcessedWorkResponse>
	{
		public async Task<ProcessedWorkResponse> HandleAsync(UploadWorkCommand command,
			CancellationToken cancellationToken)
		{
			var file = command.File;
			if (file is null) throw new ArgumentException("Work is not uploaded");

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			var memoryStream = new MemoryStream();

			await file.CopyToAsync(memoryStream, cancellationToken);

			var workEntity = new Work
			{
				FileName = Path.GetFileNameWithoutExtension(file.FileName),
				Extension = Path.GetExtension(file.FileName).ToLowerInvariant(),
				LoadDate = DateTime.UtcNow,
				Content = memoryStream.ToArray(),
				StudentId = command.StudentId
			};

			await repository.AddNewWorkAsync(workEntity);

			return workEntity.ToDto();
		}
	}
}