using BuildingBlocks.Contracts;

namespace Modules.Works.Application.Common.Models
{
	public class DeleteWorkCommand : CommandBase
	{
		public Guid workId { get; set; } = Guid.Empty!;
	}
}
