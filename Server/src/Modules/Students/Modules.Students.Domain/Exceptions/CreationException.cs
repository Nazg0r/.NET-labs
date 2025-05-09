namespace Modules.Students.Domain.Exceptions
{
	internal class CreationException(string entityName)
		: Exception($"Could not create the {entityName} entity");
}