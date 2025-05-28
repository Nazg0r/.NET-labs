namespace BuildingBlocks.Contracts
{
    public abstract class CommandBase(Guid id) : ICommand
    {
        public Guid Id { get; } = id;

        protected CommandBase() : this(Guid.NewGuid())
        {
        }
    }
}