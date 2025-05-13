using Microsoft.Extensions.DependencyInjection;

namespace Modules.Students.Application.UseCases.Login
{
	public static class LoginStudentExtensions
	{
		public static IServiceCollection AddLoginStudent(this IServiceCollection service)
		{
			service.AddScoped<LoginStudentHandler>();
			return service;
		}
	}
}
