using Microsoft.Extensions.DependencyInjection;

namespace Modules.Students.Application.UseCases.Register
{
	public static class RegisterStudentExtensions
	{
		public static IServiceCollection AddRegisterStudent(this IServiceCollection service)
		{
			service.AddScoped<RegisterStudentHandler>();
			return service;
		}
	}
}
