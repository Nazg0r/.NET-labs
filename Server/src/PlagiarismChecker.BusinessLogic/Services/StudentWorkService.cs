using Shared.Errors;
using BusinessLogic.Interfaces;
using BusinessLogic.Mappers;
using DataAccess.Interfaces;
using Shared.DTOs;
using Microsoft.AspNetCore.Http;
using DataAccess.Entities;
using System.Text;


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

		public async Task<List<PlagiarismResponseDto>> GetPercentagesAsync(Guid id)
		{
			var allWorks = await studentWorkRepository.GetAllWorksAsync();
			var selectedWork = allWorks.FirstOrDefault(w => w.Id == id);

			if (selectedWork is null) throw new StudentWorkNotFoundException(id);

			var matchingWorks = allWorks.Where(w => 
			w.Extension == selectedWork.Extension &&
			!w.Id.Equals(selectedWork.Id));

			var percentages = matchingWorks.Select(w => new PlagiarismResponseDto()
			{
				Id = w.Id,
				Name = w.FileName,
				SimilarityPercentage = CalculatePercentage(
					Encoding.UTF8.GetString(selectedWork.Content),
					Encoding.UTF8.GetString(w.Content)
					)
			})
			.OrderByDescending(p => p.SimilarityPercentage)
			.ToList();

			return percentages;
		}

		private double CalculatePercentage(string firsText, string secondText)
		{
			var firstWordDictionary = GetWordFrequency(firsText);
			var secondWordDictionary = GetWordFrequency(secondText);

			List<string> words = firstWordDictionary.Keys
				.Union(secondWordDictionary.Keys)
				.Distinct()
				.ToList();

			double dotProduct = 0;
			double firstMagnitude = 0;
			double secondMagnitude = 0;

			foreach (var word in words) 
			{
				firstWordDictionary.TryGetValue(word, out int firstCount);
				secondWordDictionary.TryGetValue(word, out int secondCount);

				dotProduct += firstCount * secondCount;
				firstMagnitude += Math.Pow(firstCount, 2);
				secondMagnitude += Math.Pow(secondCount, 2);
			}
			firstMagnitude = Math.Sqrt(firstMagnitude);
			secondMagnitude = Math.Sqrt(secondMagnitude);

			var result = firstMagnitude * secondMagnitude == 0 ? 0 :
			   (dotProduct / (firstMagnitude * secondMagnitude)) * 100;

			return Math.Round(result, 2);
		}

		private Dictionary<string, int> GetWordFrequency(string text)
		{
			var words = text.ToLower().Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\n', '\r', '\t' },
							  StringSplitOptions.RemoveEmptyEntries);

			return words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
		}
	}
}
