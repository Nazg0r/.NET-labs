namespace Modules.Students.Application.Contracts
{
	public interface IStudentQueries
	{
		public Task<Student> GetStudentByUsername(string username);
		public Student GetStudentByWorkId(Guid id);
	}
}