namespace Modules.Students.Application.Contracts
{
	public interface IStudentAuthenticator
	{
		Task<Student> ValidateCredentialsAsync(string username, string password);
	}
}
