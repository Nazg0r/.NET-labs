namespace Modules.Students.Domain.Exceptions
{
	internal class UnauthorizedException()
		: Exception("Could not confirm student identity");
}