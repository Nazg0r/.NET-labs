namespace BusinessLogic.Errors
{
	public class StudentNotFoundException(string username) : NotFoundException($"Student with username: {username}")
	{
	}
}
