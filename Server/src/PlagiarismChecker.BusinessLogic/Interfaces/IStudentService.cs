using Shared.DTOs;

namespace BusinessLogic.Interfaces
{
	public interface IStudentService
	{
		public Task<StudentResponseDto> GetStudentByUsernameAsync(string username);
		public Task<string> GetAuthorByWorkIdAsync(Guid id);

	}
}
