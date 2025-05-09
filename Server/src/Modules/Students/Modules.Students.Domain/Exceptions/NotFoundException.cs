namespace Modules.Students.Domain.Exceptions
{
	internal class NotFoundException(string item)
		: Exception($"Item {item} wasn`t found");
}