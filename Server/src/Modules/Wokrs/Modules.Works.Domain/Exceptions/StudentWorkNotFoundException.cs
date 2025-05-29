using BuildingBlocks.Exceptions;

namespace Modules.Works.Domain.Exceptions
{
    public class StudentWorkNotFoundException(Guid id)
        : NotFoundException($"`student work` with id {id}");
}