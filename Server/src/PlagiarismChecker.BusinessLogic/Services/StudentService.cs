using Shared.Errors;
using BusinessLogic.Interfaces;
using BusinessLogic.Mappers;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using Microsoft.EntityFrameworkCore;

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

		public async Task<string> GetAuthorByWorkIdAsync(Guid id)
		{
			var student = await userManager.Users.FirstOrDefaultAsync(u => u.Works!.Any(w => w.Id == id));
			if (student == null) throw new StudentNotFoundException($"username: {id}");

			return $"{student.Name} {student.Surname} {student.Group}";
		}
	}
}
