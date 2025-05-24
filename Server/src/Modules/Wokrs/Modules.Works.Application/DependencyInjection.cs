using Microsoft.Extensions.Hosting;
using Modules.Works.Application.UseCases.BulkExport;
using Modules.Works.Application.UseCases.DeleteWork;
using Modules.Works.Application.UseCases.GetAllWorks;
using Modules.Works.Application.UseCases.GetSimilarityPercentage;
using Modules.Works.Application.UseCases.GetWorkById;
using Modules.Works.Application.UseCases.UploadWork;

namespace Modules.Works.Application
{
	public static class DependencyInjection
	{
		public static void AddWorksModuleApplication(this IHostApplicationBuilder builder)
		{
			builder.Services
				.AddUploadWork()
				.AddGetWorkById()
				.AddGetSimilarityPercentage()
				.AddGetAllWorks()
				.AddDeleteWork()
				.AddBulkExport();
		}
	}
}