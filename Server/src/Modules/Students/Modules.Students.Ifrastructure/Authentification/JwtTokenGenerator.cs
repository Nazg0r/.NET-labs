using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modules.Students.Application.Contracts;
using Modules.Students.Domain.Entities;

namespace Modules.Students.Infrastructure.Authentification
{
	public class JwtTokenGenerator(IConfiguration config) :
		IJwtTokenGenerator
	{
		public string GenerateToken(Student student)
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, student.Name),
				new(ClaimTypes.Surname, student.Surname),
				new("Group", student.Group),
				new("UserName", student.Username!)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.UtcNow.AddDays(int.Parse(config["JWT:Expires"]!)),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public DateTime GetTokenExpiry()
		{
			return DateTime.UtcNow.AddDays(int.Parse(config["JWT:Expires"]!));
		}
	}
}