using BuildingBlocks.Contracts;

namespace Modules.Students.Application.Common.Models
{
	public class RegisterStudentCommand : CommandBase
	{
		public string Username { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Surname { get; set; } = null!;
		public string Group { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}