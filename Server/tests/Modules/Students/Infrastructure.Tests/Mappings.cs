using Modules.Students.Domain.Entities;
using Modules.Students.Infrastructure.Common.Mappings;
using Modules.Students.Infrastructure.Identity;
using TestsTools;

namespace Infrastructure.Tests
{
	public class Mappings
	{
		[Fact]
		public void Should_ReturnStudentIdentity_WhenCalledFromStudent()
		{
			// Arrange
			var student = SharedTestsData.TestStudents.Last();

			// Act
			var result = student.ToIdentity();
			// Assert
			Assert.NotNull(result);
			Assert.IsType<StudentIdentity>(result);
			Assert.Equal(student.Name, result.Name);
			Assert.Equal(student.Surname, result.Surname);
			Assert.Equal(student.Group, result.Group);
		}

		[Fact]
		public void Should_ReturnStudent_WhenCalledFromStudentIdentity()
		{
			// Arrange
			var identity = new StudentIdentity()
			{
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				WorksIds = []
			};

			// Act
			var result = identity.ToDomain();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<Student>(result);
			Assert.Equal(identity.Name, result.Name);
			Assert.Equal(identity.Surname, result.Surname);
			Assert.Equal(identity.Group, result.Group);
		}
	}
}
