namespace Modules.Works.Application.Contracts
{
    public interface IWorkRepository
    {
        Task<Work?> GetWorkByIdAsync(Guid id);
        Task<List<Work>> GetAllWorksAsync();
        Task<List<Work>> GetWorksByStudentIdAsync(string id);
        Task<Work> AddNewWorkAsync(Work work);
        Task<bool> DeleteWorkAsync(Guid id);
    }
}