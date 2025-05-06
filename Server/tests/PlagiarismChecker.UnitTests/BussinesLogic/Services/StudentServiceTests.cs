using BusinessLogic.Services;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.DTOs;
using Shared.Errors;

namespace PlagiarismChecker.BussinesLogic.Services
{
	public class StudentServiceTests
	{
		private readonly Mock<UserManager<Student>> _userManagerMock;

		public StudentServiceTests()
		{
			var userStoreMock = new Mock<IUserStore<Student>>();
			_userManagerMock = new Mock<UserManager<Student>>
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
		}

		public class GetStudentByUsernameAsync : StudentServiceTests
		{
			[Fact]
			public async Task Should_ReturnStudentResponseDto_WhenUserExist()
			{
				// Arrange
				var student = SharedTestsData.TestStudents.First();
				var username = student.UserName;

				_userManagerMock.Setup(x => x.FindByNameAsync(username))
					.ReturnsAsync(student);

				var studentService = new StudentService(_userManagerMock.Object);

				// Act
				var result = await studentService.GetStudentByUsernameAsync(username);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentResponseDto>(result);
				Assert.Equal(student.Id, result.Id);
				Assert.Equal(student.UserName, result.Username);
				Assert.Equal(student.Name, result.Name);
				Assert.Equal(student.Surname, result.Surname);
				Assert.Equal(student.Group, result.Group);
				Assert.Equal(student.Works!.First().Id, result.Works!.First().Id);
				_userManagerMock.Verify(m => m.FindByNameAsync(username), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentNotFoundException_WhenUserNotExist()
			{
				// Arrange
				var username = SharedTestsData.TestStudents.First().UserName!;

				_userManagerMock.Setup(x => x.FindByNameAsync(username))
					.ReturnsAsync((Student)null!);
				var studentService = new StudentService(_userManagerMock.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentNotFoundException>(() =>
					studentService.GetStudentByUsernameAsync(username));
				_userManagerMock.Verify(m => m.FindByNameAsync(username), Times.Once);
			}
		}

		public class GetAuthorByWorkIdAsync : StudentServiceTests
		{
			[Fact]
			public void Should_ReturnAuthor_WhenUserExist()
			{
				// Arrange
				var student = SharedTestsData.TestStudents.First();
				var workId = student.Works!.First().Id;

				_userManagerMock.Setup(x => x.Users)
					.Returns(new List<Student> { student }.AsQueryable());
				var studentService = new StudentService(_userManagerMock.Object);

				// Act
				var result = studentService.GetAuthorByWorkId(workId);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<string>(result);
				Assert.Equal($"{student.Name} {student.Surname} {student.Group}", result);
			}

			[Fact]
			public void Should_ThrowStudentNotFoundException_WhenUserNotExist()
			{
				// Arrange
				var workId = Guid.NewGuid();

				_userManagerMock.Setup(x => x.Users)
					.Returns(new List<Student>().AsQueryable());
				var studentService = new StudentService(_userManagerMock.Object);

				// Act & Assert
				Assert.Throws<StudentWorkNotFoundException>(() => studentService.GetAuthorByWorkId(workId));
			}
		}
	}
}