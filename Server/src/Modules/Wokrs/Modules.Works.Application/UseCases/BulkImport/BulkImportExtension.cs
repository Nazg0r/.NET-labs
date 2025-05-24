using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.BulkImport
{
	public static class BulkImportExtension
	{
		public static IServiceCollection AddBulkImport(this IServiceCollection services)
		{
			services.AddScoped<BulkImportHandler>();
			return services;
		}
	}
}
