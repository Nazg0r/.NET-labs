using BusinessLogic.Mappers;
using Shared.DTOs;

namespace PlagiarismChecker.BussinesLogic.Mappers
{
	public class EntityToDtoTests
	{
		public class ToDto : EntityToDtoTests
		{
			[Fact]
			public void Should_ReturnStudentWorkResponseDto_WhenCalledFromStudentWork()
			{
				// Arrange
				var studentWork = SharedTestsData.WorksWithSameExtension[0];

				// Act
				var result = studentWork.ToDto();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWorkResponseDto>(result);
				Assert.Equal(studentWork.Id, result.Id);
				Assert.Equal(studentWork.Content, result.Content);
				Assert.Equal(studentWork.FileName + studentWork.Extension, result.FileName);
				Assert.Equal(studentWork.LoadDate, result.LoadDate);
				Assert.Equal(studentWork.StudentId, result.StudentId);
			}

			[Fact]
			public void Should_ReturnStudentResponseDto_WhenCalledFromStudent()
			{
				// Arrange
				var student = SharedTestsData.TestStudents[0];

				// Act
				var result = student.ToDto();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentResponseDto>(result);
				Assert.Equal(student.Id, result.Id);
				Assert.Equal(student.Name, result.Name);
				Assert.Equal(student.Surname, result.Surname);
				Assert.Equal(student.Group, result.Group);
				Assert.NotNull(result.Works);
				Assert.Equal(student.Works!.Count(), result.Works.Count());
				Assert.IsType<StudentWorkDto>(result.Works.First());
			}
		}

		public class ToStudentWorkDto : EntityToDtoTests
		{
			[Fact]
			public void Should_ReturnStudentWorkDto_WhenCalledFromStudentWork()
			{
				// Arrange
				var studentWork = SharedTestsData.WorksWithSameExtension[1];

				// Act
				var result = studentWork.ToStudentWorkDto();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<StudentWorkDto>(result);
				Assert.Equal(studentWork.Id, result.Id);
				Assert.Equal(studentWork.Content, result.Content);
				Assert.Equal(studentWork.FileName + studentWork.Extension, result.FileName);
				Assert.Equal(studentWork.LoadDate, result.LoadDate);
			}
		}
	}
}