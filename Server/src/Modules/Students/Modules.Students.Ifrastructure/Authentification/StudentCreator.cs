using Microsoft.AspNetCore.Identity;
using Modules.Students.Application.Contracts;
using Modules.Students.Domain.Entities;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Common.Mappings;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Infrastructure.Authentification
{
	internal class StudentCreator(UserManager<StudentIdentity> userManager)
		: IStudentCreator
	{
		public async Task CreateAsync(Student student, string password, CancellationToken cancellationToken = default)
		{
			var result = await userManager.CreateAsync(student.ToIdentity(), password);

			if (!result.Succeeded)
				throw new StudentCreationException();
		}
	}
}