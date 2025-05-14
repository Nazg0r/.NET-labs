using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.DeleteWork
{
	public static class DeleteWorkExtension
	{
		public static IServiceCollection AddDeleteWork(this IServiceCollection services)
		{
			services.AddScoped<DeleteWorkHandler>();
			return services;
		}
	}
}