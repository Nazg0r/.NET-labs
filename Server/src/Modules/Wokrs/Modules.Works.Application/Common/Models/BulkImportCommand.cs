using BuildingBlocks.Contracts;
using Microsoft.AspNetCore.Http;

namespace Modules.Works.Application.Common.Models
{
	public class BulkImportCommand : ICommand
	{
		public Guid Id { get; } = Guid.Empty!;
		public IFormFile File { get; set; } = null!;
	}
}
