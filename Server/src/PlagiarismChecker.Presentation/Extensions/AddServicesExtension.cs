using BusinessLogic.Interfaces;
using BusinessLogic.Services;

namespace Presentation.Extensions
{
	public static class AddServicesExtension
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddSingleton<IStudentWorkService, StudentWorkService>();
			services.AddScoped<IAuthentificationService, AuthentificationService>();
			services.AddScoped<IStudentService, StudentService>();
			return services;
		}
	}
}
