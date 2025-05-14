using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.GetWorkById
{
	public static class GetWorkByIdExtension
	{
		public static IServiceCollection AddGetWorkById(this IServiceCollection services)
		{
			services.AddScoped<GetWorkByIdHandler>();
			return services;
		}	
	}
}
