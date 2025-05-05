using Shared.Errors;
using BusinessLogic.Interfaces;
using BusinessLogic.Mappers;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogic.Services
{
	public class AuthentificationService(
		UserManager<Student> userManager,
		SignInManager<Student> signInManager,
		IConfiguration configuration
		) 
		: IAuthentificationService
	{
		public async Task RegisterAsync(RegistrationRequestDto credentials)
		{
			var result = await userManager.CreateAsync(
				credentials.ToEntity(), 
				credentials.Password
				);

			if (!result.Succeeded) throw new StudentCreationException();

			return;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials)
		{
			Student? student = await userManager.FindByNameAsync(credentials.Username);

			if (student is null) throw new StudentNotFoundException($"username: {credentials.Username}");

			var result = await signInManager.CheckPasswordSignInAsync(
				student,
				credentials.Password,
				false
				);

			if (!result.Succeeded) throw new UnauthorizedException();

			var token = GenerateJwtToken(student);

			return new LoginResponseDto()
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				ExpiresDate = token.ValidTo
			};
		}

		private JwtSecurityToken GenerateJwtToken(Student student)
		{
			List<Claim> authClaims = new()
			{
				new Claim(ClaimTypes.Name, student.Name),
				new Claim(ClaimTypes.Surname, student.Surname),
				new Claim("Group", student.Group),
				new Claim("UserName", student.UserName!)
			};

			var authSigningKey =
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]??""));

			return new JwtSecurityToken(
				claims: authClaims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.Now.AddDays(int.Parse(configuration["JWT:Expires"]??"1")),
				signingCredentials: new SigningCredentials(
					authSigningKey,
					SecurityAlgorithms.HmacSha256Signature
				));
		}
	}
}
