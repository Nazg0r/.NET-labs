using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PlagiarismChecker.Controllers.Seeders
{
	public static class StudentControllerSeeder
	{
		public static async Task PrepareRegisteredStudentAsync(WebApplicationFixture fixture, Student student,
			string password)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			await userManager.CreateAsync(student, password);
		}

		public static async Task RemoveRegisteredStudentAsync(WebApplicationFixture fixture, Student student)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			await userManager.DeleteAsync(student);
		}
	}
}