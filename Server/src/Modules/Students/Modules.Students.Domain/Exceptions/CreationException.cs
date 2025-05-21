namespace Modules.Students.Domain.Exceptions
{
	public class CreationException(string entityName)
		: Exception($"Could not create the {entityName} entity");
}