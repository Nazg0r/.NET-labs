namespace Modules.Students.Application.Contracts
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(Student student);
		DateTime GetTokenExpiry();
	}
}