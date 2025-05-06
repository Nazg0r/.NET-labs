using System.Text;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using TestTools;

namespace PlagiarismChecker.DataAccess.Repositories
{
	public class StudentWorkRepositoryTests : IDisposable
	{
		private readonly DataContext _testContext;
		private readonly DbContextOptions<DataContext> _testOptions;

		public StudentWorkRepositoryTests()
		{
			_testOptions = new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.UseLazyLoadingProxies()
				.Options;
			_testContext = new DataContext(_testOptions);
			_testContext.Students.AddRange(SharedTestsData.TestStudents);
			_testContext.SaveChanges();
		}

		public class GetWorkByIdAsync : StudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnStudentWork_WhenExist()
			{
				// Arrange
				var testWork = SharedTestsData.TestStudents.First().Works![0];

				var repo = new StudentWorkRepository(_testContext);

				// Act
				var result = await repo.GetWorkByIdAsync(testWork.Id);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWork>(result);
				Assert.Equal(testWork.Id, result.Id);
				Assert.Equal(testWork.Content, result.Content);
				Assert.Equal(testWork.FileName, result.FileName);
				Assert.Equal(testWork.LoadDate, result.LoadDate);
				Assert.Equal(testWork.Extension, result.Extension);
				Assert.Equal(testWork.StudentId, result.StudentId);
			}

			[Fact]
			public async Task Should_ReturnNull_WhenWorkNotExist()
			{
				// Arrange
				var testWorkId = Guid.NewGuid();
				var repo = new StudentWorkRepository(_testContext);

				// Act
				var result = await repo.GetWorkByIdAsync(testWorkId);

				// Assert
				Assert.Null(result);
			}
		}

		public class GetAllWorksAsync : StudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnAllWorks_WhenExist()
			{
				// Arrange
				var testWorks = SharedTestsData.TestStudents.SelectMany(s => s.Works!);
				var repo = new StudentWorkRepository(_testContext);

				// Act
				var result = await repo.GetAllWorksAsync();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<StudentWork>>(result);
				Assert.Equal(testWorks.Count(), result.Count);
			}

			[Fact]
			public async Task Should_ReturnEmptyList_WhenNoWorksExist()
			{
				// Arrange
				var options = new DbContextOptionsBuilder<DataContext>()
					.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
					.Options;

				await using DataContext context = new(options);

				var repo = new StudentWorkRepository(context);

				// Act
				var result = await repo.GetAllWorksAsync();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<List<StudentWork>>(result);
				Assert.Empty(result);
			}
		}

		public class AddNewWorkAsync : StudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_AddNewWork_WhenValid()
			{
				// Arrange
				var testStudent = SharedTestsData.TestStudents.First();
				var newWork = new StudentWork
				{
					Content = Encoding.UTF8.GetBytes("<?hph>"),
					FileName = "newWork",
					LoadDate = DateTime.UtcNow,
					Extension = ".php",
					StudentId = testStudent.Id

				};
				var options = new DbContextOptionsBuilder<DataContext>()
					.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
					.Options;

				await using DataContext context = new(options);

				var repo = new StudentWorkRepository(context);

				// Act
				var result = await repo.AddNewWorkAsync(newWork);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWork>(result);
				Assert.NotEmpty(result.Id.ToString());
				Assert.Equal(newWork.Content, result.Content);
				Assert.Equal(newWork.FileName, result.FileName);
				Assert.Equal(newWork.LoadDate, result.LoadDate);
				Assert.Equal(newWork.Extension, result.Extension);
				Assert.Equal(newWork.StudentId, result.StudentId);
			}
		}

		public class DeleteWorkAsync : StudentWorkRepositoryTests
		{
			[Fact]
			public async Task Should_ReturnTrue_WhenWorkExist()
			{
				// Arrange
				var testWork = SharedTestsData.TestStudents.Last().Works![0];
				var options = new DbContextOptionsBuilder<DataContext>()
					.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
					.Options;

				await using DataContext context = new(options);

				context.Students.AddRange(SharedTestsData.TestStudents);

				var repo = new StudentWorkRepository(context);

				// Act
				var result = await repo.DeleteWorkAsync(testWork.Id);
				var deletedWork = await repo.GetWorkByIdAsync(testWork.Id);

				// Assert
				Assert.Null(deletedWork);
				Assert.True(result);
			}

			[Fact]
			public async Task Should_ReturnFalse_WhenWorkNotExist()
			{
				// Arrange
				var testWorkId = Guid.NewGuid();
				var repo = new StudentWorkRepository(_testContext);

				// Act
				var result = await repo.DeleteWorkAsync(testWorkId);

				// Assert
				Assert.False(result);
			}
		}

		public void Dispose() =>
			_testContext.Dispose();
	}
}