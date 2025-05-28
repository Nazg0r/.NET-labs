using BuildingBlocks.Contracts;
using MassTransit;
using Modules.Works.IntegrationEvents;

namespace Modules.Works.Application.UseCases.UploadWork
{
    public class UploadWorkHandler(
        IWorkRepository repository,
        IBus bus)
        : ICommandHandler<UploadWorkCommand, Work>
    {
        public async Task<Work> HandleAsync(UploadWorkCommand command,
            CancellationToken cancellationToken)
        {
            await using var file = command.FileStream;
            if (file is null) throw new ArgumentException("Work is not uploaded");

            var extension = Path.GetExtension(command.FileName).ToLowerInvariant();
            var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream, cancellationToken);

            var workEntity = new Work
            {
                FileName = Path.GetFileNameWithoutExtension(command.FileName),
                Extension = extension,
                LoadDate = DateTime.UtcNow,
                Content = memoryStream.ToArray(),
                StudentId = command.StudentId
            };

            await repository.AddNewWorkAsync(workEntity);

            await bus.Publish(new WorkUploadedEvent(workEntity.Id, workEntity.StudentId), CancellationToken.None);

            return workEntity;
        }
    }
}