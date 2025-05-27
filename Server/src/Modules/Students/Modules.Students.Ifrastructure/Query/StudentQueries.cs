using Microsoft.AspNetCore.Identity;
using Modules.Students.Application.Contracts;
using Modules.Students.Domain.Entities;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Common.Mappings;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Infrastructure.Query
{
	public class StudentQueries(UserManager<StudentIdentity> userManager) : IStudentQueries
	{
		public async Task<Student> GetStudentByUsernameAsync(string username)
		{
			var student = await userManager.FindByNameAsync(username);
			if (student is null) throw new StudentNotFoundException($"username: {username}");
			return student.ToDomain();
		}

		public Student GetStudentByWorkId(Guid id)
		{
			var student = userManager.Users
				.FirstOrDefault(u => u.WorksIds.Any(workId => workId == id));

			if (student is null) throw new StudentNotFoundException($"workId: {id}");
			return student.ToDomain();
		}

		public async Task<Student> GetStudentByIdAsync(string id)
		{
			var student = await userManager.FindByIdAsync(id);
			if (student is null) throw new StudentNotFoundException($"id: {id}");
			return student.ToDomain();
		}
	}
}