using DataAccess.Entities;
using PlagiarismChecker.Controllers.Seeders;
using Shared.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PlagiarismChecker.Controllers
{
	public class StudentControllerTests : IClassFixture<WebApplicationFixture>
	{
		private readonly WebApplicationFixture _fixture;
		private readonly HttpClient _client;

		public StudentControllerTests(WebApplicationFixture fixture)
		{
			_fixture = fixture;
			_client = fixture.CreateClient();
		}

		[Fact]
		public async Task GetStudentByUsername_ShouldReturnOk_WithStudentResponseDto_WhenUserExist()
		{
			Student? student = null;
			try
			{
				// Arrange
				var username = "johnDoe";
				student = new Student
				{
					UserName = username,
					Name = "John",
					Surname = "Doe",
					Group = "IM-00"
				};
				await StudentControllerSeeder.PrepareRegisteredStudentAsync(_fixture, student, "qwertyui");

				// Act
				var response = await _client.GetAsync($"/api/student/{username}");

				// Assert
				response.EnsureSuccessStatusCode();
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);

				var result = await response.Content.ReadFromJsonAsync<StudentResponseDto>();
				Assert.NotNull(result);
				Assert.Equal(student.UserName, result.Username);
				Assert.Equal(student.Name, result.Name);
				Assert.Equal(student.Surname, result.Surname);
				Assert.Equal(student.Group, result.Group);
			}
			finally
			{
				if (student is not null)
					await StudentControllerSeeder.RemoveRegisteredStudentAsync(_fixture, student);
			}
		}

		[Fact]
		public async Task GetStudentByUsername_ShouldReturnNotFound_WhenUserNotExist()
		{
			// Arrange
			var username = "johnDoe";

			// Act
			var response = await _client.GetAsync($"/api/student/{username}");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Contains($"Student with username: {username} not found", await response.Content.ReadAsStringAsync());
		}

		[Fact]
		public async Task GetAuthorByWorkId_ShouldReturnOk_WithString_WhenWorkExist()
		{
			Guid workId = Guid.NewGuid();
			Student? student = null;
			try
			{
				// Arrange
				student = new Student
				{
					UserName = "johnDoe",
					Name = "John",
					Surname = "Doe",
					Group = "IM-00",
					Works = new List<StudentWork>
					{
						new StudentWork
						{
							Id = workId,
							Content = Encoding.UTF8.GetBytes("hello world"),
							FileName = "test",
							LoadDate = DateTime.UtcNow,
							Extension = ".cs"
						}
					}
				};

				await StudentControllerSeeder.PrepareRegisteredStudentAsync(_fixture, student, "qwertyui");

				// Act
				var response = await _client.GetAsync($"/api/student/work/{workId}");

				// Assert
				response.EnsureSuccessStatusCode();
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				var result = await response.Content.ReadAsStringAsync();
				Assert.NotNull(result);
				Assert.Equal($"{student.Name} {student.Surname} {student.Group}", result);
			}
			finally
			{
				if (student is not null)
					await StudentControllerSeeder.RemoveRegisteredStudentAsync(_fixture, student);
			}
		}

		[Fact]
		public async Task GetAuthorByWorkId_ShouldReturnNotFound_WhenWorkNotExist()
		{
			// Arrange
			var workId = Guid.NewGuid();

			// Act
			var response = await _client.GetAsync($"/api/student/work/{workId}");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Contains($"Item `student work` with id {workId} wasn`t found",
				await response.Content.ReadAsStringAsync());
		}
	}
}