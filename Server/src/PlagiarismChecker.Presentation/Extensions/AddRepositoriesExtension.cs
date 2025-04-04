using DataAccess.Interfaces;
using DataAccess.Repositories;

namespace Presentation.Extensions
{
	public static class AddRepositoriesExtension
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services) 
		{
			services.AddSingleton<IStudentWorkRepository, StudentWorkRepository>();
			return services;
		}
	}
}
