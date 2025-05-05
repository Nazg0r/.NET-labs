using DataAccess.Entities;
using Shared.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace PlagiarismChecker.Controllers
{
	public class StudentController : IClassFixture<StudentControllerFixture>
	{
		private readonly StudentControllerFixture _fixture;
		private readonly HttpClient _client;
		public StudentController(StudentControllerFixture fixture)
		{
			_fixture = fixture;
			_client = fixture.CreateClient();
		}

		[Fact]
		public async Task Register_ShouldReturnCreated_WhenValidRequest()
		{
			// Arrange
			var request = new
			{
				Username = "testuser",
				Name = "Test",
				Surname = "User",
				Group = "IM-00",
				Password = "password123"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}

		[Theory]
		[MemberData(
			nameof(StudentControllerSeeder.RegisterValidationCases),
			MemberType = typeof(StudentControllerSeeder))]
		public async Task Register_ShouldReturnBadRequest_WhenInvalidRequest(
			string username, string name, string surname, string group, string password, string errMessage)
		{
			// Arrange
			var request = new
			{
				Username = username,
				Name = name,
				Surname = surname,
				Group = group,
				Password = password
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains(errMessage, await response.Content.ReadAsStringAsync());
		}

		[Fact]
		public async Task Login_ShouldReturnOk_WhenRequestValid_AndUserRegistered()
		{
			Student? student = null;
			try
			{
				student = new()
				{
					UserName = "johnDoe",
					Name = "John",
					Surname = "Doe",
					Group = "IM-00"
				};
				// Arrange
				var password = "qwertyui";

				await StudentControllerSeeder.PrepareRegisteredStudentAsync(_fixture, student, password);

				// Act
				var response = await _client.PostAsJsonAsync("/api/student/login", new
				{
					Username = student.UserName,
					Password = password
				});

				// Assert
				response.EnsureSuccessStatusCode();
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);

				var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
				Assert.NotNull(result);
				Assert.False(string.IsNullOrEmpty(result.Token));
				Assert.True(result.ExpiresDate > DateTime.Now);
			}
			finally
			{
				if (student is not null)
				await StudentControllerSeeder.RemoveRegisteredStudentAsync(_fixture, student);
			}


		}

		[Fact]
		public async Task Login_ShouldReturnNotFound_WhenRequestValid_AndUserNotRegistered()
		{
			// Arrange
			LoginRequestDto request = new()
			{
				Username = "johnDoe",
				Password = "qwertyui"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Contains($"Student with username: {request.Username} not found", await response.Content.ReadAsStringAsync());
		}

		[Theory]
		[MemberData(
			nameof(StudentControllerSeeder.LoginValidationCases),
			MemberType = typeof(StudentControllerSeeder))]
		public async Task Login_ShouldReturnBadRequest_WhenInvalidRequest(string username, string password, string errMessage)
		{
			// Arrange
			LoginRequestDto request = new()
			{
				Username = username,
				Password = password
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains(errMessage, await response.Content.ReadAsStringAsync());
		}
	}
}
