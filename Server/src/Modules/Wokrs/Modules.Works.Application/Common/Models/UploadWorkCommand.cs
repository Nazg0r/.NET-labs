using Microsoft.AspNetCore.Http;

namespace Modules.Works.Application.Common.Models
{
	public class UploadWorkCommand : CommandBase
	{
		public IFormFile File { get; set; } = null!;
		public string StudentId { get; set; } = null!;
	}
}