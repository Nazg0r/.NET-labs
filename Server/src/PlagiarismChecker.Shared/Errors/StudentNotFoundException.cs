namespace Shared.Errors
{
	public class StudentNotFoundException(string username) : NotFoundException($"Student with username: {username}")
	{
	}
}
