namespace BuildingBlocks.Contracts
{
	public interface IQueryHandler<in TQuery, TResponse>
	{
		public Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
	}
}