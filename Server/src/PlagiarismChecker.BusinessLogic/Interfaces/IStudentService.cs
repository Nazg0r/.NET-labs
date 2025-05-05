using Shared.DTOs;

namespace BusinessLogic.Interfaces
{
	public interface IStudentService
	{
		public Task<StudentResponseDto> GetStudentByUsernameAsync(string username);
		public string GetAuthorByWorkId(Guid id);
	}
}
