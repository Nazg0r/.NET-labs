using BuildingBlocks.Contracts;

namespace Modules.Works.Application.Common.Models
{
	public class UploadWorkCommand : CommandBase
	{
		public Stream FileStream { get; set; } = null!;
		public string FileName { get; init; } = null!;
		public string StudentId { get; set; } = null!;
	}
}