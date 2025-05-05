using Shared.Errors;
using BusinessLogic.Interfaces;
using BusinessLogic.Mappers;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;

namespace BusinessLogic.Services
{
	public class StudentService(UserManager<Student> userManager)
		: IStudentService
	{
		public async Task<StudentResponseDto> GetStudentByUsernameAsync(string username)
		{
			var student = await userManager.FindByNameAsync(username);
			if (student == null)  throw new StudentNotFoundException($"username: { username }");

			return student.ToDto();
		}

		public string GetAuthorByWorkId(Guid id)
		{
			var student = userManager.Users.FirstOrDefault(u => u.Works!.Any(w => w.Id == id));
			if (student == null) throw new StudentNotFoundException($"username: {id}");

			return $"{student.Name} {student.Surname} {student.Group}";
		}
	}
}
