namespace Modules.Students.Application.Common.Models
{
	public class LoginStudentResponse
	{
		public string Token { get; set; } = null!;
		public DateTime ExpiresDate { get; set; }
	}
}