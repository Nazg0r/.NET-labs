namespace Modules.Works.Domain.Exceptions
{
	public class NotFoundException(string item) 
		: Exception($"Item {item} wasn`t found");
}
