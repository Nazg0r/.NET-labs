using BusinessLogic.Mappers;
using DataAccess.Entities;
using Shared.DTOs;

namespace PlagiarismChecker.BussinesLogic.Mappers
{
	public class DtoToEntityTests
	{
		public class ToEntity : DtoToEntityTests
		{
			[Fact]
			public void Should_ReturnStudent_WhenCalledFromRegistrationRequestDto()
			{
				// Arrange
				var dto = new RegistrationRequestDto
				{
					Username = "testUser",
					Name = "Test",
					Surname = "User",
					Group = "TestGroup"
				};

				// Act
				var result = dto.ToEntity();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<Student>(result);
				Assert.Equal(dto.Username, result.UserName);
				Assert.Equal(dto.Name, result.Name);
				Assert.Equal(dto.Surname, result.Surname);
				Assert.Equal(dto.Group, result.Group);
			}
		}
	}
}
