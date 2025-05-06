using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PlagiarismChecker.Controllers.Seeders
{
	public static class StudentWorkControllerSeeder
	{
		public static async Task PrepareStudentWorksAsync(WebApplicationFixture fixture)
		{
			var password = "qwertyui";
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			foreach (var student in SharedTestsData.TestStudents)
			{
				if (await userManager.FindByNameAsync(student.UserName) is null)
				{
					await userManager.CreateAsync(student, password);
				}
			}
		}

		public static async Task RemoveStudentWorksAsync(WebApplicationFixture fixture)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			foreach (var student in SharedTestsData.TestStudents)
			{
				await userManager.DeleteAsync(student);
			}
		}
	}
}