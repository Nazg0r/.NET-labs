namespace Modules.Students.Application.Contracts
{
	public interface IStudentQueries
	{
		public Task<Student> GetStudentByUsernameAsync(string username);
		public Student GetStudentByWorkId(Guid id);
		public Task<Student> GetStudentByIdAsync(string id);
	}
}