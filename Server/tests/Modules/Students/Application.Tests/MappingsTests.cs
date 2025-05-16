using Modules.Students.Application.Common.Mappings;
using Modules.Students.Application.Common.Models;
using Modules.Students.Domain.Entities;
using TestsTools;

namespace Application.Tests
{
	public class MappingsTests
	{
		[Fact]
		public void Should_ReturnStudent_WhenCalledFromRegisterStudentCommand()
		{
			// Arrange
			var command = new RegisterStudentCommand
			{
				Username = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = "IM-12",
				Password = "qwerty"
			};

			// Act
			var result = command.ToEntity();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<Student>(result);
			Assert.Equal(command.Username, result.Username);
			Assert.Equal(command.Name, result.Name);
			Assert.Equal(command.Surname, result.Surname);
			Assert.Equal(command.Group, result.Group);
		}

		[Fact]
		public void Should_ReturnGetStudentByUsernameResponse_WhenCalledFromStudent()
		{
			// Arrange
			var student = SharedTestsData.TestStudents.Last();

			// Act
			var result = student.ToDto();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<GetStudentByUsernameResponse>(result);
			Assert.Equal(student.Id.ToString(), result.Id);
			Assert.Equal(student.Username, result.Username);
			Assert.Equal(student.Name, result.Name);
			Assert.Equal(student.Surname, result.Surname);
			Assert.Equal(student.Group, result.Group);
		}
	}
}
