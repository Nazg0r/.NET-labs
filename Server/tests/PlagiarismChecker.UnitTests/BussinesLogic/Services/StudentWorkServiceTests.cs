using BusinessLogic.Services;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Shared.DTOs;
using Shared.Errors;
using System.Text;
using TestTools;

namespace PlagiarismChecker.BussinesLogic.Services
{
	public class StudentWorkServiceTests
	{
		StudentWork StubStudentWork = new()
		{
			Id = Guid.NewGuid(),
			Content = Encoding.UTF8.GetBytes("hello world"),
			FileName = "homework",
			Extension = ".cs",
			LoadDate = DateTime.Now,
			StudentId = Guid.NewGuid().ToString()
		};

		public class GetWorkAsync : StudentWorkServiceTests
		{
			[Fact]
			public async Task Should_ReturnStudentWorkResponseDto_WhenWorkIsExistAsync()
			{
				// Arrange
				Guid workId = StubStudentWork.Id;
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetWorkByIdAsync(workId)).ReturnsAsync(StubStudentWork);

				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act 
				var result = await studentWorkService.GetWorkAsync(workId);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWorkResponseDto>(result);
				Assert.Equal(workId, result.Id);
				Assert.Equal("homework.cs", result.FileName);
				Assert.Equal(Encoding.UTF8.GetBytes("hello world"), result.Content);
				mockRepo.Verify(r => r.GetWorkByIdAsync(workId), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentWorkNotFoundException_WhenWorkIsNotExist()
			{
				// Arrange
				Guid workId = Guid.NewGuid();
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetWorkByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StudentWork?)null);
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				//Act & Assert
				await Assert.ThrowsAsync<StudentWorkNotFoundException>(async () =>
					await studentWorkService.GetWorkAsync(workId));
				mockRepo.Verify(r => r.GetWorkByIdAsync(workId), Times.Once);
			}
		}

		public class GetWorksAsync : StudentWorkServiceTests
		{
			[Fact]
			public async Task Should_ReturnListOfStudentWorkResponseDto_WhenWorksAreExist()
			{
				// Arrange
				var works = new List<StudentWork> { StubStudentWork };
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act

				var result = await studentWorkService.GetWorksAsync();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<StudentWorkResponseDto>>(result);
				Assert.Single(result);
				Assert.Equal("homework.cs", result.First().FileName);
				mockRepo.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentWorksNotFoundException_WhenWorksAreNotExist()
			{
				// Arrange
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(new List<StudentWork>());
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentWorksNotFoundException>(async () =>
					await studentWorkService.GetWorksAsync());
				mockRepo.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}
		}

		public class StoreWorkAsync : StudentWorkServiceTests
		{
			[Fact]
			public async Task Should_ReturnStudentWorkResponseDto_WhenWorkSuccessfullyStoredAsync()
			{
				// Arrange
				var filename = "homework.cs";
				var content = Encoding.UTF8.GetBytes("hello world");
				var stream = new MemoryStream(content);
				var studentId = Guid.NewGuid().ToString();

				var mockFile = new Mock<IFormFile>();
				mockFile.Setup(f => f.FileName).Returns(filename);
				mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => { stream.CopyTo(s); });

				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.AddNewWorkAsync(It.IsAny<StudentWork>()));

				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act
				var result = await studentWorkService.StoreWorkAsync(mockFile.Object, studentId);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWorkResponseDto>(result);
				Assert.Equal(filename, result.FileName);
				Assert.Equal(content, result.Content);
				Assert.Equal(studentId, result.StudentId);
				Assert.True(result.LoadDate <= DateTime.UtcNow);

				mockRepo.Verify(r => r.AddNewWorkAsync(It.IsAny<StudentWork>()), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowArgumentExceptionError_WhenFileIsNull()
			{
				// Arrange
				IFormFile? file = null;
				var studentId = Guid.NewGuid().ToString();
				var mockRepo = new Mock<IStudentWorkRepository>();
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act & Assert
				await Assert.ThrowsAsync<ArgumentException>(async () =>
					await studentWorkService.StoreWorkAsync(file!, studentId));
				mockRepo.Verify(r => r.AddNewWorkAsync(It.IsAny<StudentWork>()), Times.Never);
			}
		}

		public class DeleteWorkAsync : StudentWorkServiceTests
		{
			[Fact]
			public async Task Should_DeleteStudentWork_WhenWorkItExist()
			{
				// Arrange
				var workId = StubStudentWork.Id;
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.DeleteWorkAsync(workId)).ReturnsAsync(true);
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act
				await studentWorkService.DeleteWorkAsync(workId);

				// Assert
				mockRepo.Verify(r => r.DeleteWorkAsync(workId), Times.Once);
			}
		}

		public class GetPercentagesAsync : StudentWorkServiceTests
		{
			[Fact]
			public async Task Should_ReturnListOfThreePlagiarismResponseDto_WhenFourWorksStored_WithTheSameExtension()
			{
				// Arrange
				var selectedWork = SharedTestsData.WorksWithSameExtension.First();
				var works = new List<StudentWork>(SharedTestsData.WorksWithSameExtension);
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act
				var result = await studentWorkService.GetPercentagesAsync(selectedWork.Id);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<PlagiarismResponseDto>>(result);
				Assert.Equal(3, result.Count);
				Assert.True(result[0].SimilarityPercentage >= result[1].SimilarityPercentage);
				Assert.Equal(SharedTestsData.WorksWithSameExtension[1].Id, result.First().Id);
				Assert.Equal(SharedTestsData.WorksWithSameExtension[2].Id, result.Last().Id);
				Assert.DoesNotContain(result, r => r.Id == selectedWork.Id);
			}

			[Fact]
			public async Task Should_ReturnListOfOnePlagiarismResponseDto_WhenFourWorksStored_WithDifferentExtension()
			{
				// Arrange
				var selectedWork = SharedTestsData.WorksWithDifferentExtension.First();
				var works = new List<StudentWork>(SharedTestsData.WorksWithDifferentExtension);
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act
				var result = await studentWorkService.GetPercentagesAsync(selectedWork.Id);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<PlagiarismResponseDto>>(result);
				Assert.Single(result);
				Assert.Equal(SharedTestsData.WorksWithDifferentExtension[2].Id, result.First().Id);
				Assert.Collection(result, r => r.Name.Contains(selectedWork.Extension));
				Assert.DoesNotContain(result, r => r.Id == selectedWork.Id);
			}

			[Fact]
			public async Task Should_ThrowStudentWorkNotFoundException_WhenWorkIsNotExist()
			{
				// Arrange
				var workId = Guid.NewGuid();
				var mockRepo = new Mock<IStudentWorkRepository>();
				mockRepo.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(new List<StudentWork>());
				var studentWorkService = new StudentWorkService(mockRepo.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentWorkNotFoundException>(async () =>
					await studentWorkService.GetPercentagesAsync(workId));
				mockRepo.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}
		}
	}
}