namespace Modules.Works.Application.Contracts
{
    public interface IBulkOperations
    {
        Task InsertAsync(IEnumerable<Work> works, CancellationToken token);
        Task<byte[]> ExportAsync(CancellationToken token);

    }
}