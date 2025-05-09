namespace Modules.Students.Domain.Exceptions
{
	internal class StudentNotFoundException(string identity)
		: NotFoundException($"Student with {identity} not found");
}