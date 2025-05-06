using System.Text;
using System.Text.Json;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TestTools;

namespace PlagiarismChecker.DataAccess.Repositories
{
	public class CachedStudentWorkRepositoryTests
	{
		private readonly Mock<IStudentWorkRepository> _mockStudentWorkRepository = new();
		private readonly Mock<IDistributedCache> _mockDistributedCache = new();

		public class GetAllWorksAsync : CachedStudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnAllWorks()
			{
				// Arrange
				var works = SharedTestsData
					.TestStudents.SelectMany(s => s.Works!).ToList();

				_mockStudentWorkRepository.Setup(repo => repo.GetAllWorksAsync())
					.ReturnsAsync(works);

				var cachedRepository =
					new CachedStudentWorkRepository(_mockStudentWorkRepository.Object, _mockDistributedCache.Object);
				// Act
				var result = await cachedRepository.GetAllWorksAsync();

				// Assert
				Assert.NotNull(result);
				Assert.Equal(works, result);
				_mockStudentWorkRepository.Verify(r => r.GetAllWorksAsync(), Times.Once);
			}
		}

		public class GetWorkByIdAsync : CachedStudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnWork_WhenWorkExistsInCache()
			{
				// Arrange
				var work = SharedTestsData.TestStudents.First().Works!.First();
				var workId = work.Id.ToString();
				var json = JsonSerializer.Serialize(work);

				_mockDistributedCache.Setup(cache => cache.GetAsync(workId, It.IsAny<CancellationToken>()))
					.ReturnsAsync(Encoding.UTF8.GetBytes(json));
				var cachedRepository =
					new CachedStudentWorkRepository(_mockStudentWorkRepository.Object, _mockDistributedCache.Object);

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
				_mockStudentWorkRepository.Verify(r => r.GetWorkByIdAsync(work.Id), Times.Never);
			}

			[Fact]
			public async Task Should_ReturnWork_WhenWorkNotExistsInCache()
			{
				// Arrange
				var work = SharedTestsData.TestStudents.First().Works![1];
				var workId = work.Id.ToString();
				var json = JsonSerializer.Serialize(work);

				_mockDistributedCache.Setup(cache => cache.GetAsync(workId, It.IsAny<CancellationToken>()))
					.ReturnsAsync((byte[])null);

				_mockStudentWorkRepository.Setup(repo => repo.GetWorkByIdAsync(work.Id))
					.ReturnsAsync(work);
				_mockDistributedCache.Setup(cache => cache.SetAsync(workId, Encoding.UTF8.GetBytes(json),
						It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);
				var cachedRepository =
					new CachedStudentWorkRepository(_mockStudentWorkRepository.Object, _mockDistributedCache.Object);

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
				_mockStudentWorkRepository.Verify(r => r.GetWorkByIdAsync(work.Id), Times.Once);
			}
		}

		public class AddNewWorkAsync : CachedStudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnStudentWork_AndWriteItToCache()
			{
				// Arrange
				var work = SharedTestsData.TestStudents.First().Works![2];
				var workId = work.Id.ToString();
				var json = JsonSerializer.Serialize(work);

				_mockStudentWorkRepository.Setup(repo => repo.AddNewWorkAsync(work))
					.ReturnsAsync(work);
				_mockDistributedCache.Setup(cache => cache.SetAsync(workId, Encoding.UTF8.GetBytes(json),
						It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

				var cachedRepository =
					new CachedStudentWorkRepository(_mockStudentWorkRepository.Object, _mockDistributedCache.Object);

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
				_mockStudentWorkRepository.Verify(r => r.AddNewWorkAsync(work), Times.Once);
			}
		}

		public class DeleteWorkAsync : CachedStudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_DeleteWorkFromCache_AndReturnTrue()
			{
				// Arrange
				var work = SharedTestsData.TestStudents.First().Works![3];
				var workId = work.Id.ToString();

				_mockDistributedCache.Setup(cache => cache.RemoveAsync(workId, It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);
				_mockStudentWorkRepository.Setup(repo => repo.DeleteWorkAsync(work.Id))
					.ReturnsAsync(true);

				var cachedRepository =
					new CachedStudentWorkRepository(_mockStudentWorkRepository.Object, _mockDistributedCache.Object);

				// Act
				var result = await cachedRepository.DeleteWorkAsync(work.Id);

				// Assert
				Assert.True(result);
				_mockDistributedCache.Verify(r => r.RemoveAsync(workId, It.IsAny<CancellationToken>()), Times.Once);
				_mockStudentWorkRepository.Verify(r => r.DeleteWorkAsync(work.Id), Times.Once);
			}
		}
	}
}