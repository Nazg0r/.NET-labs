using Microsoft.Extensions.DependencyInjection;

namespace Modules.Students.Application.UseCases.GetStudentByWorkId
{
	public static class GetAuthorByWorkIdExtension
	{
		public static IServiceCollection AddGetStudentByWorkId(this IServiceCollection service)
		{
			service.AddScoped<GetAuthorByWorkIdHandler>();
			return service;
		}
	}
}