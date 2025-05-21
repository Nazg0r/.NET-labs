using Microsoft.AspNetCore.Identity;
using Modules.Students.Application.Contracts;
using Modules.Students.Domain.Entities;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Common.Mappings;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Infrastructure.Authentification
{
	public class IdentityStudentAuthenticator(
		UserManager<StudentIdentity> userManager,
		SignInManager<StudentIdentity> signInManager)
		: IStudentAuthenticator
	{
		public async Task<Student> ValidateCredentialsAsync(string username, string password)
		{
			var studentIdentity = await userManager.FindByNameAsync(username);
			if (studentIdentity is null)
				throw new StudentNotFoundException($"username: {username}");

			var result = await signInManager.CheckPasswordSignInAsync(studentIdentity, password, false);
			if (!result.Succeeded)
				throw new UnauthorizedException();

			return studentIdentity.ToDomain();
		}
	}
}