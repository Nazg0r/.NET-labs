namespace Modules.Students.Application.Common.Models
{
	public class LoginStudentCommand : CommandBase
	{
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}