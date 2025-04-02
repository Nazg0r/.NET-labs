using System.IdentityModel.Tokens.Jwt;

namespace Shared.DTOs
{
	public class LoginResponseDto
	{
		public string Token { get; set; } = default!;
		public DateTime ExpiresDate { get; set; }
	}
}
