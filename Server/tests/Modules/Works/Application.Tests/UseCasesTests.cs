using Microsoft.AspNetCore.Http;
using Modules.Works.Application.Common.Models;
using Modules.Works.Application.Contracts;
using Modules.Works.Application.UseCases.DeleteWork;
using Modules.Works.Application.UseCases.GetAllWorks;
using Modules.Works.Application.UseCases.GetWorkById;
using Modules.Works.Application.UseCases.UploadWork;
using Modules.Works.Domain.Entities;
using Modules.Works.Domain.Exceptions;
using Moq;
using System.Text;
using BuildingBlocks.Models;
using MassTransit;
using Modules.Students.IntegrationEvents;
using Modules.Works.Application.UseCases;
using Modules.Works.IntegrationEvents;
using TestsTools;
using Modules.Works.Application.UseCases.GetSimilarityPercentage;

namespace Application.Tests
{
	public class UseCasesTests
	{
		private readonly Mock<IWorkRepository> _workRepositoryMock;

		public UseCasesTests()
		{
			_workRepositoryMock = new Mock<IWorkRepository>();
		}

		public class DeleteWorkTests : UseCasesTests
		{
			[Fact]
			public async Task Should_DeleteStudentWork_WhenWorkItExist()
			{
				// Arrange
				var workId = Guid.NewGuid();
				_workRepositoryMock.Setup(r => r.DeleteWorkAsync(workId)).ReturnsAsync(true);
				var handler = new DeleteWorkHandler(_workRepositoryMock.Object);

				// Act
				await handler.HandleAsync(new DeleteWorkCommand() { workId = workId }, CancellationToken.None);

				// Assert
				_workRepositoryMock.Verify(r => r.DeleteWorkAsync(workId), Times.Once);
			}
		}

		public class GetAllWorksTests : UseCasesTests
		{
			[Fact]
			public async Task Should_ReturnListOfStudentWorkResponseDto_WhenWorksAreExist()
			{
				// Arrange
				var works = SharedTestsData.WorksWithSameExtension;

				_workRepositoryMock.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var handler = new GetAllWorksHandler(_workRepositoryMock.Object);

				// Act

				var result = await handler.HandleAsync(Unit.Value, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<ProcessedWorkResponse>>(result);
				Assert.Equal(works.Count, result.Count);
				_workRepositoryMock.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentWorksNotFoundException_WhenWorksAreNotExist()
			{
				// Arrange
				_workRepositoryMock.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(new List<Work>());
				var handler = new GetAllWorksHandler(_workRepositoryMock.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentWorksNotFoundException>(async () =>
					await handler.HandleAsync(Unit.Value, CancellationToken.None));
				_workRepositoryMock.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}
		}

		public class GetWorkByIdTests : UseCasesTests
		{
			[Fact]
			public async Task Should_ReturnStudentWorkResponseDto_WhenWorkIsExistAsync()
			{
				// Arrange
				Work work = SharedTestsData.WorksWithDifferentExtension.First();

				_workRepositoryMock.Setup(r => r.GetWorkByIdAsync(work.Id)).ReturnsAsync(work);

				var handler = new GetWorkByIdHandler(_workRepositoryMock.Object);

				// Act 
				var result = await handler.HandleAsync(work.Id, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<ProcessedWorkResponse>(result);
				Assert.Equal(work.Id, result.Id);
				Assert.Equal("original.cs", result.FileName);
				Assert.Equal(Encoding.UTF8.GetBytes("home work"), result.Content);
				_workRepositoryMock.Verify(r => r.GetWorkByIdAsync(work.Id), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentWorkNotFoundException_WhenWorkIsNotExist()
			{
				// Arrange
				Guid workId = Guid.NewGuid();

				_workRepositoryMock.Setup(r => r.GetWorkByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Work?)null);

				var handler = new GetWorkByIdHandler(_workRepositoryMock.Object);

				//Act & Assert
				await Assert.ThrowsAsync<StudentWorkNotFoundException>(async () =>
					await handler.HandleAsync(workId, CancellationToken.None));
				_workRepositoryMock.Verify(r => r.GetWorkByIdAsync(workId), Times.Once);
			}
		}

		public class UploadWorkTests : UseCasesTests
		{
			private readonly Mock<IBus> _mockBus;

			public UploadWorkTests()
			{
				_mockBus = new Mock<IBus>();
			}

			[Fact]
			public async Task Should_ReturnStudentWorkResponseDto_WhenWorkSuccessfullyStored()
			{
				// Arrange
				var filename = "homework.cs";
				var content = Encoding.UTF8.GetBytes("hello world");
				var stream = new MemoryStream(content);
				var studentId = Guid.NewGuid().ToString();

				var mockFile = new Mock<IFormFile>();
				mockFile.Setup(f => f.FileName).Returns(filename);
				mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
					.Callback<Stream, CancellationToken>((s, _) => { stream.CopyTo(s); });

				_mockBus.Setup(b => b.Publish(It.IsAny<StudentWorkDto>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

				_workRepositoryMock.Setup(r => r.AddNewWorkAsync(It.IsAny<Work>()));

				var command = new UploadWorkCommand
				{
					File = mockFile.Object,
					StudentId = studentId
				};

				var handler = new UploadWorkHandler(_workRepositoryMock.Object, _mockBus.Object);

				// Act
				var result = await handler.HandleAsync(command, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<ProcessedWorkResponse>(result);
				Assert.Equal(filename, result.FileName);
				Assert.Equal(content, result.Content);
				Assert.Equal(studentId, result.StudentId);
				Assert.True(result.LoadDate <= DateTime.UtcNow);

				_workRepositoryMock.Verify(r => r.AddNewWorkAsync(It.IsAny<Work>()), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowArgumentExceptionError_WhenFileIsNull()
			{
				// Arrange
				IFormFile? file = null;
				var studentId = Guid.NewGuid().ToString();

				var command = new UploadWorkCommand
				{
					File = file!,
					StudentId = studentId
				};

				var handler = new UploadWorkHandler(_workRepositoryMock.Object, _mockBus.Object);

				// Act & Assert
				await Assert.ThrowsAsync<ArgumentException>(async () =>
					await handler.HandleAsync(command, CancellationToken.None));
				_workRepositoryMock.Verify(r => r.AddNewWorkAsync(It.IsAny<Work>()), Times.Never);
			}
		}

		public class GetSimilarityPercentage : UseCasesTests
		{
			[Fact]
			public async Task Should_ReturnListOfFourPlagiarismResponseDto_WhenFourWorksStored_WithTheSameExtension()
			{
				// Arrange
				var selectedWork = SharedTestsData.WorksWithSameExtension.First();
				var works = new List<Work>(SharedTestsData.WorksWithSameExtension);

				_workRepositoryMock.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var handler = new GetSimilarityPercentageHandler(_workRepositoryMock.Object);

				// Act
				var result = await handler.HandleAsync(selectedWork.Id, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<GetSimilarityPercentageResponse>>(result);
				Assert.Equal(4, result.Count);
				Assert.True(result[0].SimilarityPercentage >= result[1].SimilarityPercentage);
				Assert.Equal(SharedTestsData.WorksWithSameExtension[3].Id, result.First().Id);
				Assert.Equal(SharedTestsData.WorksWithSameExtension[2].Id, result.Last().Id);
				Assert.DoesNotContain(result, r => r.Id == selectedWork.Id);
				_workRepositoryMock.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}

			[Fact]
			public async Task Should_ReturnListOfOnePlagiarismResponseDto_WhenFourWorksStored_WithDifferentExtension()
			{
				// Arrange
				var selectedWork = SharedTestsData.WorksWithDifferentExtension.First();
				var works = new List<Work>(SharedTestsData.WorksWithDifferentExtension);

				_workRepositoryMock.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(works);
				var handler = new GetSimilarityPercentageHandler(_workRepositoryMock.Object);

				// Act
				var result = await handler.HandleAsync(selectedWork.Id, CancellationToken.None);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<GetSimilarityPercentageResponse>>(result);
				Assert.Single(result);
				Assert.Equal(SharedTestsData.WorksWithDifferentExtension[2].Id, result.First().Id);
				Assert.Collection(result, r => r.Name.Contains(selectedWork.Extension));
				Assert.DoesNotContain(result, r => r.Id == selectedWork.Id);
				_workRepositoryMock.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentWorkNotFoundException_WhenWorkIsNotExist()
			{
				// Arrange
				var workId = Guid.NewGuid();

				_workRepositoryMock.Setup(r => r.GetAllWorksAsync()).ReturnsAsync(new List<Work>());
				var handler = new GetSimilarityPercentageHandler(_workRepositoryMock.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentWorkNotFoundException>(async () =>
					await handler.HandleAsync(workId, CancellationToken.None));
				_workRepositoryMock.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}
		}
	}
}