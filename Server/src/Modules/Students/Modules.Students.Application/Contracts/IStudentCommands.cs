namespace Modules.Students.Application.Contracts
{
    public interface IStudentCommands
    {
        public Task AddWorkIdToStudent(string studentId, Guid workId);
    }
}