namespace Modules.Works.Application.Contracts
{
	public interface IQueryHandler<in TQuery, TResponse>
	{
		public Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
	}
}