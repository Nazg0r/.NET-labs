using Microsoft.AspNetCore.Http;
using Shared.DTOs;
using static BusinessLogic.Services.StudentWorkService;

namespace BusinessLogic.Interfaces
{
	public interface IStudentWorkService
	{
		Task<StudentWorkResponseDto> GetWorkAsync(Guid id);
		Task<List<StudentWorkResponseDto>> GetWorksAsync();
		Task<StudentWorkResponseDto> StoreWorkAsync(IFormFile work, string studentId);
		Task DeleteWorkAsync(Guid id);
		Task<List<PlagiarismResponseDto>> GetPercentagesAsync(Guid id);
	}
}
