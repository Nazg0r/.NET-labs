namespace Shared.Errors
{
	public class StudentNotFoundException(string identity) : NotFoundException($"Student with {identity} not found")
	{
	}
}
