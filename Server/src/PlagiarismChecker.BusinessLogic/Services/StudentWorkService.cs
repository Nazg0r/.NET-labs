using BusinessLogic.Errors;
using BusinessLogic.Interfaces;
using BusinessLogic.Mappers;
using DataAccess.Interfaces;
using Shared.DTOs;
using Microsoft.AspNetCore.Http;
using DataAccess.Entities;


namespace BusinessLogic.Services
{
	public class StudentWorkService(IStudentWorkRepository studentWorkRepository)
		: IStudentWorkService
	{
		public async Task<StudentWorkResponseDto> GetWorkAsync(Guid id)
		{
			var work = await studentWorkRepository.GetWorkByIdAsync(id);
			if (work is null) throw new StudentWorkNotFoundException(id);

			return work.ToDto();
		}
		public async Task<List<StudentWorkResponseDto>> GetWorksAsync()
		{
			var works = await studentWorkRepository.GetAllWorksAsync();
			if (!works.Any()) throw new StudentWorksNotFoundException();

			return works.Select(w => w.ToDto()).ToList();
		}
		public async Task<StudentWorkResponseDto> StoreWorkAsync(IFormFile file, string studentId)
		{
			if (file is null) throw new ArgumentException("Work is not uploaded");

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			var memoryStream = new MemoryStream();

			file.CopyTo(memoryStream);

			var workEntity = new StudentWork
			{
				FileName = Path.GetFileNameWithoutExtension(file.FileName),
				Extension = Path.GetExtension(file.FileName).ToLowerInvariant(),
				LoadDate = DateTime.UtcNow,
				Content = memoryStream.ToArray(),
				StudentId = studentId

			};

			await studentWorkRepository.AddNewWorkAsync(workEntity);

			return workEntity.ToDto();
		}

		public async Task DeleteWorkAsync(Guid id)
		{
			var result = await studentWorkRepository.DeleteWorkAsync(id);

			return;
		}
	}
}
