using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.GetUploadedWork
{
	public static class GetUploadedWorkExtension
	{
		public static IServiceCollection AddGetUploadedWork(this IServiceCollection services)
		{
			services.AddScoped<GetUploadedWorkHandler>();
			return services;
		}
	}
}