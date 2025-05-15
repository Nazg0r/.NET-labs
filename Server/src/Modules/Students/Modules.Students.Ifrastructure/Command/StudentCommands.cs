using Microsoft.AspNetCore.Identity;
using Modules.Students.Application.Contracts;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Infrastructure.Command
{
	public class StudentCommands(UserManager<StudentIdentity> userManager) : IStudentCommands
	{
		public async Task AddWorkIdToStudent(string studentId, Guid workId)
		{
			var student = await userManager.FindByIdAsync(studentId);
			if (student is null) throw new StudentNotFoundException($"id: {studentId}");

			student.WorksIds.Add(workId);

			var result = await userManager.UpdateAsync(student);
			if (!result.Succeeded) throw new StudentUpdatingException($"id: {studentId}");

			await userManager.UpdateAsync(student);
		}
	}
}