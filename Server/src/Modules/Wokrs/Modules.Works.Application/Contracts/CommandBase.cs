namespace Modules.Works.Application.Contracts
{
	public abstract class CommandBase(Guid id) : ICommand
	{
		public Guid Id { get; } = id;

		protected CommandBase() : this(Guid.NewGuid())
		{
		}
	}
}