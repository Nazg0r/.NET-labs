using Microsoft.AspNetCore.Identity;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Command;
using Modules.Students.Infrastructure.Identity;
using Moq;

namespace Infrastructure.Tests
{
	public class CommandTests
	{
		private readonly Mock<UserManager<StudentIdentity>> _mockUserManager;

		public CommandTests()
		{
			_mockUserManager = GetUserManagerMock();
		}

		public class StudentCommandsTests : CommandTests
		{
			[Fact]
			public async Task Should_AddWorkIdToStudent_WhenStudentExists()
			{
				// Arrange
				var studentId = "teststudentid";
				var workId = Guid.NewGuid();
				var studentIdentity = new StudentIdentity
				{
					Id = studentId,
					UserName = "testuser",
					Name = "Test",
					Surname = "User",
					Group = "TestGroup",
					WorksIds = new List<Guid>()
				};

				_mockUserManager.Setup(x =>
					x.FindByIdAsync(studentId)).ReturnsAsync(studentIdentity);

				_mockUserManager.Setup(x =>
					x.UpdateAsync(It.IsAny<StudentIdentity>())).ReturnsAsync(IdentityResult.Success);

				var studentCommands = new StudentCommands(_mockUserManager.Object);

				// Act
				await studentCommands.AddWorkIdToStudent(studentId, workId);

				// Assert
				Assert.Contains(workId, studentIdentity.WorksIds);
				Assert.Single(studentIdentity.WorksIds);
				_mockUserManager.Verify(x => x.FindByIdAsync(studentId), Times.Once);
				_mockUserManager.Verify(x => x.UpdateAsync(studentIdentity), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentNotFoundException_WhenStudentNotExists()
			{
				// Arrange
				var studentId = "teststudentid";
				var workId = Guid.NewGuid();
				StudentIdentity? studentIdentity = null;

				_mockUserManager.Setup(x =>
					x.FindByIdAsync(studentId)).ReturnsAsync(studentIdentity);

				var studentCommands = new StudentCommands(_mockUserManager.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
					await studentCommands.AddWorkIdToStudent(studentId, workId));
			}

			[Fact]
			public async Task Should_ThrowStudentUpdatingException_WhenUpdateDataFailed()
			{
				// Arrange
				var studentId = "teststudentid";
				var workId = Guid.NewGuid();
				var studentIdentity = new StudentIdentity
				{
					Id = studentId,
					UserName = "testuser",
					Name = "Test",
					Surname = "User",
					Group = "TestGroup",
					WorksIds = new List<Guid>()
				};

				_mockUserManager.Setup(x =>
					x.FindByIdAsync(studentId)).ReturnsAsync(studentIdentity);

				_mockUserManager.Setup(x =>
					x.UpdateAsync(It.IsAny<StudentIdentity>())).ReturnsAsync(IdentityResult.Failed());

				var studentCommands = new StudentCommands(_mockUserManager.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentUpdatingException>(async () =>
					await studentCommands.AddWorkIdToStudent(studentId, workId));
			}
		}

		private Mock<UserManager<StudentIdentity>> GetUserManagerMock()
		{
			var userStoreMock = new Mock<IUserStore<StudentIdentity>>();
			var userManagerMock = new Mock<UserManager<StudentIdentity>>
			(
				userStoreMock.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);
			return userManagerMock;
		}
	}
}