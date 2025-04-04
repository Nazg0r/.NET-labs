using Microsoft.AspNetCore.Http;
using Shared.DTOs;

namespace BusinessLogic.Interfaces
{
	public interface IStudentWorkService
	{
		Task<StudentWorkResponseDto> GetWorkAsync(Guid id);
		Task<List<StudentWorkResponseDto>> GetWorksAsync();
		Task<StudentWorkResponseDto> StoreWorkAsync(IFormFile work, string studentId);
		Task DeleteWorkAsync(Guid id);
	}
}
