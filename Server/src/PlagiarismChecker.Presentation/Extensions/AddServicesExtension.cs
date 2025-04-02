using BusinessLogic.Interfaces;
using BusinessLogic.Services;

namespace Presentation.Extensions
{
	public static class AddServicesExtension
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<IAuthentificationService, AuthentificationService>();
			return services;
		}
	}
}
