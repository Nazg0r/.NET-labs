using MassTransit;
using Modules.Students.Application.Common.Models;
using System.Net;
using System.Net.Http.Json;
using TestsTools;

namespace API.EndToEndTests;

public class ValidationTests(WebApplicationFixture fixture) : IClassFixture<WebApplicationFixture>
{
	private readonly WebApplicationFixture _fixture = fixture;
	private readonly HttpClient _client = fixture.CreateClient();


	public class Register(WebApplicationFixture fixture) : ValidationTests(fixture)
	{
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

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenInvalidUsername()
		{
			// Arrange
			var request = new
			{
				Username = "john",
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				Password = "qwertyui",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Username must be at least 6 character long", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenUsernameEmpty()
		{
			// Arrange
			var request = new
			{
				Username = "",
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				Password = "qwertyui",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Username is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenNameEmpty()
		{
			// Arrange
			var request = new
			{
				Username = "johnDoe",
				Name = "",
				Surname = "Doe",
				Group = "IM-00",
				Password = "qwertyui",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Name is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenGroupEmpty()
		{
			// Arrange
			var request = new
			{
				Username = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = "",
				Password = "qwertyui",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Group is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Theory]
		[InlineData("IM-0")]
		[InlineData("GROUP-999")]
		public async Task Register_ShouldReturnBadRequest_WhenGroupHasInvalidFormat(string group)
		{
			// Arrange
			var request = new
			{
				Username = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = group,
				Password = "qwertyui",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Incorrect group format", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenPasswordEmpty()
		{
			// Arrange
			var request = new
			{
				Username = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				Password = "",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Password is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenPasswordLesThen8Chars()
		{
			// Arrange
			var request = new
			{
				Username = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				Password = "qwerty",
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/register", request);

			// Assert
			var content = await response.Content.ReadAsStringAsync();
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Password must be at least 8 character long", content);
			Assert.Contains("ValidationException", content);
		}
	}

	public class Login(WebApplicationFixture fixture) : ValidationTests(fixture)
	{
		[Fact]
		public async Task Login_ShouldReturnOk_WhenRequestValid_AndUserRegistered()
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
			await _client.PostAsJsonAsync("/api/student/register", request);
			var response = await _client.PostAsJsonAsync("/api/student/login", new
			{
				request.Username,
				request.Password
			});

			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var result = await response.Content.ReadFromJsonAsync<LoginStudentResponse>();
			Assert.NotNull(result);
			Assert.False(string.IsNullOrEmpty(result.Token));
			Assert.True(result.ExpiresDate > DateTime.Now);
		}

		[Fact]
		public async Task Login_ShouldReturnNotFound_WhenRequestValid_AndUserNotRegistered()
		{
			// Arrange
			LoginStudentCommand request = new()
			{
				Username = "johnDoe2",
				Password = "qwertyui"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);
			var content = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Contains($"Student with username: {request.Username} not found", content);
		}

		[Fact]
		public async Task Login_ShouldReturnBadRequest_WhenUsernameEmpty()
		{
			// Arrange
			LoginStudentCommand request = new()
			{
				Username = "",
				Password = "qwertyui"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);
			var content = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Username is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Login_ShouldReturnBadRequest_WhenPasswordEmpty()
		{
			// Arrange
			LoginStudentCommand request = new()
			{
				Username = "johnDoe",
				Password = ""
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);
			var content = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Password is required", content);
			Assert.Contains("ValidationException", content);
		}

		[Fact]
		public async Task Login_ShouldReturnBadRequest_WhenPasswordLesThen8Chars()
		{
			// Arrange
			LoginStudentCommand request = new()
			{
				Username = "johnDoe",
				Password = "qwerty"
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/student/login", request);
			var content = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains("Password must be at least 8 character long", content);
			Assert.Contains("ValidationException", content);
		}
	}
}
