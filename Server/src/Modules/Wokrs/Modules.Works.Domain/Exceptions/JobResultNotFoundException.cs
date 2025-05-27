using BuildingBlocks.Exceptions;

namespace Modules.Works.Domain.Exceptions
{
	public class JobResultNotFoundException(string jobId)
		: NotFoundException($"JobResult with jobId: \"{jobId}\"");
}