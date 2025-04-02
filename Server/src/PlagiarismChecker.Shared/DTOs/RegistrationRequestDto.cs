namespace Shared.DTOs
{
	public class RegistrationRequestDto
	{
		public string Username { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string Surname { get; set; } = default!;
		public string Group { get; set; } = default!;
		public string Password { get; set; } = default!;
	}
}
