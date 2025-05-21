using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.UploadWork
{
	public static class UploadWorkExtension
	{
		public static IServiceCollection AddUploadWork(this IServiceCollection services)
		{
			services.AddScoped<UploadWorkHandler>();
			return services;
		}	
	}
}
