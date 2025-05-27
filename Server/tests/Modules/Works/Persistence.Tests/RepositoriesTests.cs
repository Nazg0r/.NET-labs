using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using Modules.Works.Persistence.Data;
using Modules.Works.Persistence.Repositories;
using Moq;
using System.Text;
using System.Text.Json;
using TestsTools;

namespace Persistence.Tests;

public class RepositoriesTests : IDisposable, IAsyncDisposable
{
	private readonly WorkDbContext _testContext;
	private readonly DbContextOptions<WorkDbContext> _testOptions;

	public RepositoriesTests()
	{
		_testOptions = new DbContextOptionsBuilder<WorkDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;
		_testContext = new WorkDbContext(_testOptions);
		_testContext.Works.AddRange(SharedTestsData.WorksWithDifferentExtension);
		_testContext.SaveChanges();
	}

	public class WorkRepositoryTests : RepositoriesTests
	{
		[Fact]
		public async Task GetWorkByIdAsync_Should_ReturnStudentWork_WhenExist()
		{
			// Arrange
			var testWork = SharedTestsData.WorksWithDifferentExtension.First();

			var repo = new WorkRepository(_testContext);

			// Act
			var result = await repo.GetWorkByIdAsync(testWork.Id);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<Work>(result);
			Assert.Equal(testWork.Id, result.Id);
			Assert.Equal(testWork.Content, result.Content);
			Assert.Equal(testWork.FileName, result.FileName);
			Assert.Equal(testWork.LoadDate, result.LoadDate);
			Assert.Equal(testWork.Extension, result.Extension);
			Assert.Equal(testWork.StudentId, result.StudentId);
		}

		[Fact]
		public async Task GetWorkByIdAsync_Should_ReturnNull_WhenWorkNotExist()
		{
			// Arrange
			var testWorkId = Guid.NewGuid();
			var repo = new WorkRepository(_testContext);

			// Act
			var result = await repo.GetWorkByIdAsync(testWorkId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetAllWorksAsync_Should_ReturnAllWorks_WhenExist()
		{
			// Arrange
			var testWorks = SharedTestsData.WorksWithDifferentExtension;
			var repo = new WorkRepository(_testContext);

			// Act
			var result = await repo.GetAllWorksAsync();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<List<Work>>(result);
			Assert.Equal(testWorks.Count(), result.Count);
		}

		[Fact]
		public async Task GetAllWorksAsync_Should_ReturnEmptyList_WhenNoWorksExist()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<WorkDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			await using WorkDbContext context = new(options);

			var repo = new WorkRepository(context);

			// Act
			var result = await repo.GetAllWorksAsync();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<List<Work>>(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetWorksByStudentIdAsync_Should_ReturnStudentWorks_WhenExist()
		{
			// Arrange
			var testStudent = SharedTestsData.TestStudents.First();
			var testWorks = SharedTestsData.WorksWithDifferentExtension
				.Where(w => w.StudentId == testStudent.Id.ToString()).ToList();
			var repo = new WorkRepository(_testContext);

			// Act
			var result = await repo.GetWorksByStudentIdAsync(testStudent.Id.ToString());

			// Assert
			Assert.NotNull(result);
			Assert.IsType<List<Work>>(result);
			Assert.Equal(testWorks.Count(), result.Count);
		}

		[Fact]
		public async Task AddNewWorkAsync_Should_AddNewWork_WhenValid()
		{
			// Arrange
			var testStudent = SharedTestsData.TestStudents.First();
			var newWork = new Work
			{
				Content = Encoding.UTF8.GetBytes("<?hph>"),
				FileName = "newWork",
				LoadDate = DateTime.UtcNow,
				Extension = ".php",
				StudentId = testStudent.Id.ToString()
			};
			var options = new DbContextOptionsBuilder<WorkDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			await using WorkDbContext context = new(options);

			var repo = new WorkRepository(context);

			// Act
			var result = await repo.AddNewWorkAsync(newWork);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<Work>(result);
			Assert.NotEmpty(result.Id.ToString());
			Assert.Equal(newWork.Content, result.Content);
			Assert.Equal(newWork.FileName, result.FileName);
			Assert.Equal(newWork.LoadDate, result.LoadDate);
			Assert.Equal(newWork.Extension, result.Extension);
			Assert.Equal(newWork.StudentId, result.StudentId);
		}
	}

	public class CachedWorkRepositoryTests : RepositoriesTests
	{
		private readonly Mock<IWorkRepository> _mockWorkRepository = new();
		private readonly Mock<IDistributedCache> _mockDistributedCache = new();

		[Fact]
		public async Task GetAllWorksAsync_Should_ReturnAllWorks()
		{
			// Arrange
			var works = SharedTestsData.WorksWithDifferentExtension;

			_mockWorkRepository.Setup(repo => repo.GetAllWorksAsync())
				.ReturnsAsync(works);

			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);
			// Act
			var result = await cachedRepository.GetAllWorksAsync();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(works, result);
			_mockWorkRepository.Verify(r => r.GetAllWorksAsync(), Times.Once);
		}

		[Fact]
		public async Task GetWorkByIdAsync_Should_ReturnWork_WhenWorkExistsInCache()
		{
			// Arrange
			var work = SharedTestsData.WorksWithDifferentExtension.First();
			var workId = work.Id.ToString();
			var json = JsonSerializer.Serialize(work);

			_mockDistributedCache.Setup(cache => cache.GetAsync(workId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(Encoding.UTF8.GetBytes(json));
			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);

			// Act
			var result = await cachedRepository.GetWorkByIdAsync(work.Id);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(work.Id, result.Id);
			Assert.Equal(work.FileName, result.FileName);
			Assert.Equal(work.Extension, result.Extension);
			Assert.Equal(work.LoadDate, result.LoadDate);
			Assert.Equal(work.Content, result.Content);
			Assert.Equal(work.StudentId, result.StudentId);
			_mockDistributedCache.Verify(r => r.GetAsync(workId, It.IsAny<CancellationToken>()), Times.Once);
			_mockWorkRepository.Verify(r => r.GetWorkByIdAsync(work.Id), Times.Never);
		}

		[Fact]
		public async Task GetWorkByIdAsync_Should_ReturnWork_WhenWorkNotExistsInCache()
		{
			// Arrange
			var work = SharedTestsData.WorksWithDifferentExtension[1];
			var workId = work.Id.ToString();
			var json = JsonSerializer.Serialize(work);

			_mockDistributedCache.Setup(cache => cache.GetAsync(workId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((byte[])null);

			_mockWorkRepository.Setup(repo => repo.GetWorkByIdAsync(work.Id))
				.ReturnsAsync(work);
			_mockDistributedCache.Setup(cache => cache.SetAsync(workId, Encoding.UTF8.GetBytes(json),
					It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);

			// Act
			var result = await cachedRepository.GetWorkByIdAsync(work.Id);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(work.Id, result.Id);
			Assert.Equal(work.FileName, result.FileName);
			Assert.Equal(work.Extension, result.Extension);
			Assert.Equal(work.LoadDate, result.LoadDate);
			Assert.Equal(work.Content, result.Content);
			Assert.Equal(work.StudentId, result.StudentId);
			_mockDistributedCache.Verify(r => r.GetAsync(workId, It.IsAny<CancellationToken>()), Times.Once);
			_mockDistributedCache.Verify(r =>
				r.SetAsync(workId, Encoding.UTF8.GetBytes(json), It.IsAny<DistributedCacheEntryOptions>(),
					It.IsAny<CancellationToken>()), Times.Once);
			_mockWorkRepository.Verify(r => r.GetWorkByIdAsync(work.Id), Times.Once);
		}

		[Fact]
		public async Task GetWorksByStudentIdAsync_Should_ReturnStudentWorks()
		{
			// Arrange
			var studentId = SharedTestsData.TestStudents.First().Id.ToString();
			var works = SharedTestsData.WorksWithDifferentExtension
				.Where(w => w.StudentId == studentId).ToList();

			_mockWorkRepository.Setup(repo => repo.GetWorksByStudentIdAsync(studentId))
				.ReturnsAsync(works);

			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);

			// Act
			var result = await cachedRepository.GetWorksByStudentIdAsync(studentId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(works, result);
			_mockWorkRepository.Verify(r => r.GetWorksByStudentIdAsync(studentId), Times.Once);
		}

		[Fact]
		public async Task AddNewWorkAsync_Should_ReturnStudentWork_AndWriteItToCache()
		{
			// Arrange
			var work = SharedTestsData.WorksWithDifferentExtension[2];
			var workId = work.Id.ToString();
			var json = JsonSerializer.Serialize(work);

			_mockWorkRepository.Setup(repo => repo.AddNewWorkAsync(work))
				.ReturnsAsync(work);
			_mockDistributedCache.Setup(cache => cache.SetAsync(workId, Encoding.UTF8.GetBytes(json),
					It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);

			// Act
			var result = await cachedRepository.AddNewWorkAsync(work);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(work.Id, result.Id);
			Assert.Equal(work.FileName, result.FileName);
			Assert.Equal(work.Extension, result.Extension);
			Assert.Equal(work.LoadDate, result.LoadDate);
			Assert.Equal(work.Content, result.Content);
			Assert.Equal(work.StudentId, result.StudentId);
			_mockDistributedCache.Verify(r =>
				r.SetAsync(workId, Encoding.UTF8.GetBytes(json), It.IsAny<DistributedCacheEntryOptions>(),
					It.IsAny<CancellationToken>()), Times.Once);
			_mockWorkRepository.Verify(r => r.AddNewWorkAsync(work), Times.Once);
		}

		[Fact]
		public async Task DeleteWorkAsync_Should_DeleteWorkFromCache_AndReturnTrue()
		{
			// Arrange
			var work = SharedTestsData.WorksWithDifferentExtension[3];
			var workId = work.Id.ToString();

			_mockDistributedCache.Setup(cache => cache.RemoveAsync(workId, It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			_mockWorkRepository.Setup(repo => repo.DeleteWorkAsync(work.Id))
				.ReturnsAsync(true);

			var cachedRepository =
				new CachedWorkRepository(_mockWorkRepository.Object, _mockDistributedCache.Object);

			// Act
			var result = await cachedRepository.DeleteWorkAsync(work.Id);

			// Assert
			Assert.True(result);
			_mockDistributedCache.Verify(r => r.RemoveAsync(workId, It.IsAny<CancellationToken>()), Times.Once);
			_mockWorkRepository.Verify(r => r.DeleteWorkAsync(work.Id), Times.Once);
		}
	}

	public void Dispose() =>
		_testContext.Dispose();

	public async ValueTask DisposeAsync() =>
		await _testContext.DisposeAsync();
}