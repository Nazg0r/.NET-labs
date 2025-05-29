namespace Modules.Students.Application.Contracts
{
    public interface IStudentCreator
    {
        Task CreateAsync(Student student, string password, CancellationToken cancellationToken = default);
    }
}