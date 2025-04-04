using BusinessLogic.Errors;
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
			if (student == null)  throw new StudentNotFoundException(username);

			return student.ToDto();
		}
	}
}
