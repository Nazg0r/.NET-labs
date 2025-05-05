using BusinessLogic.Services;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.DTOs;
using Shared.Errors;
using System.Text;

namespace PlagiarismChecker.BussinesLogic.Services
{
	public class StudentServiceTests
	{
		Student TestStudent = new Student
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "johnDoe",
			Name = "John",
			Surname = "Doe",
			Group = "IM-01",
			Works = new List<StudentWork>
			{
				new StudentWork
				{
					Id = Guid.NewGuid(),
					Content = Encoding.UTF8.GetBytes("hello world"),
					FileName = "test.txt",
					LoadDate = DateTime.UtcNow,
					Extension = ".txt"
				}
			}
		};

		public class GetStudentByUsernameAsync : StudentServiceTests
		{
			[Fact]
			public async Task Should_ReturnStudentResponseDto_WhenUserExist()
			{
				// Arrange
				var username = TestStudent.UserName;
				var student = TestStudent;

				var mockUserManager = GetUserManagerMock();
				mockUserManager.Setup(x => x.FindByNameAsync(username))
					.ReturnsAsync(student);

				var studentService = new StudentService(mockUserManager.Object);

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
				mockUserManager.Verify(m => m.FindByNameAsync(username), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentNotFoundException_WhenUserNotExist()
			{
				// Arrange
				var username = TestStudent.UserName!;
				var mockUserManager = GetUserManagerMock();

				mockUserManager.Setup(x => x.FindByNameAsync(username))
					.ReturnsAsync((Student)null!);
				var studentService = new StudentService(mockUserManager.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentNotFoundException>(() => studentService.GetStudentByUsernameAsync(username));
				mockUserManager.Verify(m => m.FindByNameAsync(username), Times.Once);
			}
		}

		public class GetAuthorByWorkIdAsync : StudentServiceTests
		{
			[Fact]
			public void Should_ReturnAuthor_WhenUserExist()
			{
				// Arrange
				var workId = TestStudent.Works!.First().Id;
				var student = TestStudent;
				var mockUserManager = GetUserManagerMock();

				mockUserManager.Setup(x => x.Users)
					.Returns(new List<Student> { student }.AsQueryable());
				var studentService = new StudentService(mockUserManager.Object);

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
				var mockUserManager = GetUserManagerMock();

				mockUserManager.Setup(x => x.Users)
					.Returns(new List<Student>().AsQueryable());
				var studentService = new StudentService(mockUserManager.Object);

				// Act & Assert
				Assert.Throws<StudentNotFoundException>(() => studentService.GetAuthorByWorkId(workId));
			}
		}

		private Mock<UserManager<Student>> GetUserManagerMock()
		{
			var userStoreMock = new Mock<IUserStore<Student>>();
			var userManagerMock = new Mock<UserManager<Student>>
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
