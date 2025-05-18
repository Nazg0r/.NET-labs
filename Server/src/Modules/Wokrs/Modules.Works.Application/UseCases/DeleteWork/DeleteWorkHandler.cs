using BuildingBlocks.Contracts;

namespace Modules.Works.Application.UseCases.DeleteWork
{
	public class DeleteWorkHandler(IWorkRepository repository) : ICommandHandler<DeleteWorkCommand, bool>
	{
		public async Task<bool> HandleAsync(DeleteWorkCommand command, CancellationToken cancellationToken)
		{
			return await repository.DeleteWorkAsync(command.workId);
		}
	}
}
