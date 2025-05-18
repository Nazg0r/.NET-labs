using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Modules.Students.Domain.Entities;
using Modules.Students.Infrastructure.Common.Mappings;
using Modules.Students.Infrastructure.Identity;
using TestsTools;

namespace API.IntegrationTests.Seeders
{
	public static class StudentSeeder
	{
		public static async Task<Student> PrepareRegisteredStudentAsync(WebApplicationFixture fixture, Student student,
			string password)
		{
			var identity = student.ToIdentity();
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StudentIdentity>>();
			await userManager.CreateAsync(identity, password);
			return identity.ToDomain();
		}

		public static async Task<Student> AddWorkIdToStudentAsync(WebApplicationFixture fixture, Student student,
			Guid workId)
		{
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StudentIdentity>>();
			var identity = await userManager.FindByIdAsync(student.Id.ToString());
			identity!.WorksIds.Add(workId);
			await userManager.UpdateAsync(identity);
			return identity.ToDomain();
		}

		public static async Task RemoveRegisteredStudentAsync(WebApplicationFixture fixture, Student student)
		{
			var identity = student.ToIdentity();
			using var scope = fixture.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StudentIdentity>>();
			await userManager.DeleteAsync(identity);
		}
	}
}