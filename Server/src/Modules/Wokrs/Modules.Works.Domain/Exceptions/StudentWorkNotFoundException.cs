namespace Modules.Works.Domain.Exceptions
{
	internal class StudentWorkNotFoundException(Guid id)
		: NotFoundException($"`student work` with id {id}");
}
