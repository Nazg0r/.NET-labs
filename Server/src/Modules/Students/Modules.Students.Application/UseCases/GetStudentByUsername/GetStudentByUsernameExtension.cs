using Microsoft.Extensions.DependencyInjection;

namespace Modules.Students.Application.UseCases.GetStudentByUsername
{
	public static class GetStudentByUsernameExtension
	{
		public static IServiceCollection AddGetStudentByUsername(this IServiceCollection service)
		{
			service.AddScoped<GetStudentByUsernameHandler>();
			return service;
		}
	}
}