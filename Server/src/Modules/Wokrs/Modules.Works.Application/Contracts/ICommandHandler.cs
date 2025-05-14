namespace Modules.Works.Application.Contracts
{
	public interface ICommandHandler<in TCommand>
		where TCommand : ICommand
	{
		public Task HandleAsync(TCommand command, CancellationToken cancellationToken);
	}

	public interface ICommandHandler<in TCommand, TResult>
		where TCommand : ICommand
	{
		public Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
	}
}
