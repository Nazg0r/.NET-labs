using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace PlagiarismChecker.Controllers
{
	public static class StudentControllerSeeder
	{
		public static async Task PrepareRegisteredStudentAsync(StudentControllerFixture fixture, Student student, string password)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			await userManager.CreateAsync(student, password);
		}
		public static async Task RemoveRegisteredStudentAsync(StudentControllerFixture fixture, Student student)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
			await userManager.DeleteAsync(student);
		}

		public static TheoryData<string, string, string> LoginValidationCases() => new()
		{
			{ "", "qwertyui", "Username is required"},
			{ "johnDoe", "", "Password is requiered" },
			{ "johnDoe", "qwerty", "Password must be at least 8 character long"}
		};

		public static TheoryData<string, string, string, string, string, string> RegisterValidationCases() => new()
		{
			{
				"john",
				"John",
				"Doe",
				"IM-00",
				"qwertyui",
				"Username must be at least 6 character long"
			},
			{
				"",
				"John",
				"Doe",
				"IM-00",
				"qwertyui",
				"Username is required"
			},
			{
				"johnDoe",
				"",
				"Doe",
				"IM-00",
				"qwertyui",
				"Name is required"
			},
			{
				"johnDoe",
				"John",
				"",
				"IM-00",
				"qwertyui",
				"Surname is required"
			},
			{
				"johnDoe",
				"John",
				"Doe",
				"",
				"qwertyui",
				"Group is required"
			},
			{
				"johnDoe",
				"John",
				"Doe",
				"IM-0",
				"qwertyui",
				"Incorect group format"
			},
			{
				"johnDoe",
				"John",
				"Doe",
				"GROUP-999",
				"qwertyui",
				"Incorect group format"
			},
			{
				"johnDoe",
				"John",
				"Doe",
				"IM-00",
				"",
				"Password is required"
			},
			{
				"johnDoe",
				"John",
				"Doe",
				"IM-00",
				"qwerty",
				"Password must be at least 8 character long"
			},
		};
	}
}
