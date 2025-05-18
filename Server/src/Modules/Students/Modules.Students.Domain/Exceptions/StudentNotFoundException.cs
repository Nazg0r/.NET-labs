using BuildingBlocks.Exceptions;

namespace Modules.Students.Domain.Exceptions
{
	public class StudentNotFoundException(string identity)
		: NotFoundException($"Student with {identity} not found");
}