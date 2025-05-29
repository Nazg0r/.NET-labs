using BuildingBlocks.Exceptions;

namespace Modules.Works.Domain.Exceptions
{
    public class StudentWorksNotFoundException()
        : NotFoundException($"`student works`");
}