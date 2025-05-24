using Microsoft.AspNetCore.Http;

namespace Modules.Works.Application.Contracts
{
	public interface ICsvParser
	{
		public List<Work> ParseCsv(IFormFile file);
	}
}
