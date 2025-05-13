namespace Modules.Students.Domain.Exceptions
{
	public class NotFoundException(string item)
		: Exception($"Item {item} wasn`t found");
}