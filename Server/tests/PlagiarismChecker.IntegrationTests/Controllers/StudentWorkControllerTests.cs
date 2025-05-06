using Shared.DTOs;
using System.Net.Http.Json;
using System.Net;
using DataAccess.Entities;
using FluentAssertions;
using System.Text;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using PlagiarismChecker.Controllers.Seeders;

namespace PlagiarismChecker.Controllers
{
	public class StudentWorkControllerTests : IClassFixture<WebApplicationFixture>
	{
		private readonly WebApplicationFixture _fixture;
		private readonly HttpClient _client;

		public StudentWorkControllerTests(WebApplicationFixture fixture, ITestOutputHelper output)
		{
			_fixture = fixture;
			_client = fixture.CreateClient();
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(2)]
		public async Task GetWork_ShouldReturnOk_WithStudentWorkResponseDto_WhenWorkExist(int workId)
		{
			// Arrange
			StudentWork? studentWork = SharedTestsData.TestStudents.FirstOrDefault()?.Works?[workId];
			var studentWorkId = Guid.NewGuid();

			await StudentWorkControllerSeeder.PrepareStudentWorksAsync(_fixture);

			// Act
			var response = await _client.GetAsync($"/api/studentwork/{studentWork?.Id}");

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var result = await response.Content.ReadFromJsonAsync<StudentWorkResponseDto>();
			Assert.Equal(studentWork?.Id, result?.Id);
			Assert.Equal(studentWork?.Content, result?.Content);
			Assert.Equal(studentWork?.FileName + studentWork?.Extension, result?.FileName);
			Assert.Equal(studentWork?.LoadDate, result?.LoadDate);
			Assert.Equal(studentWork?.StudentId, result?.StudentId);
		}

		[Fact]
		public async Task GetWork_ShouldReturnNotFound_WhenWorkNotExist()
		{
			// Arrange
			var studentWorkId = Guid.NewGuid();

			// Act
			var response = await _client.GetAsync($"/api/studentwork/{studentWorkId}");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task GetAllStudentWorks_ShouldReturnOk_WithListStudentWorkResponseDto_WhenWorksExist()
		{
			// Arrange
			List<StudentWork> studentWorks = SharedTestsData.TestStudents.SelectMany(s => s.Works!).ToList();
			List<StudentWorkResponseDto> expected = studentWorks.Select(w => new StudentWorkResponseDto
			{
				Id = w.Id,
				Content = w.Content,
				FileName = w.FileName + w.Extension,
				LoadDate = w.LoadDate,
				StudentId = w.StudentId
			}).ToList();

			await StudentWorkControllerSeeder.PrepareStudentWorksAsync(_fixture);

			// Act
			var response = await _client.GetAsync("/api/studentwork");

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var result = await response.Content.ReadFromJsonAsync<List<StudentWorkResponseDto>>();
			Assert.NotNull(result);
			expected.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task GetAllStudentWorks_ShouldReturnNotFound_WhenWorksNotExist()
		{
			// Arrange
			await StudentWorkControllerSeeder.RemoveStudentWorksAsync(_fixture);

			// Act
			var response = await _client.GetAsync("/api/studentwork");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task StoreWork_ShouldReturnOk_WithStudentWorkResponseDto_WhenWorkIsStored()
		{
			// Arrange
			var student = SharedTestsData.TestStudents.FirstOrDefault();
			var fileName = "test.txt";
			var fileContent = "Hello, World!";
			var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
			var file = new StreamContent(fileStream);
			file.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var content = new MultipartFormDataContent
			{
				{ file, "file", fileName }
			};
			await StudentWorkControllerSeeder.PrepareStudentWorksAsync(_fixture);

			// Act
			var response = await _client.PostAsync($"/api/studentwork/upload/{student?.Id}", content);

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var result = await response.Content.ReadFromJsonAsync<StudentWorkResponseDto>();
			Assert.NotNull(result);
			Assert.Equal(fileName, result?.FileName);
			Assert.Equal(fileContent, Encoding.UTF8.GetString(result?.Content!));
			Assert.Equal(student?.Id, result?.StudentId);
		}

		[Fact]
		public async Task StoreWork_ShouldReturnBadRequest_WhenFileIsNotProvided()
		{
			// Arrange
			var student = SharedTestsData.TestStudents.FirstOrDefault();
			var content = new MultipartFormDataContent();

			// Act
			var response = await _client.PostAsync($"/api/studentwork/upload/{student?.Id}", content);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task DeleteWork_ShouldReturnNoContent_WhenWorkIsDeleted()
		{
			// Arrange
			var studentWork = SharedTestsData.TestStudents.LastOrDefault()?.Works?.LastOrDefault();
			await StudentWorkControllerSeeder.PrepareStudentWorksAsync(_fixture);

			// Act
			var response = await _client.DeleteAsync($"/api/studentwork/{studentWork?.Id}");

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task DeleteWork_ShouldReturnNoContent_WhenWorkNotExist()
		{
			// Arrange
			var studentWorkId = Guid.NewGuid();
			// Act
			var response = await _client.DeleteAsync($"/api/studentwork/{studentWorkId}");
			// Assert
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task GetPlagiarismPercentages_ShouldReturnOk_WithListPlagiarismResponseDto_WhenWorkExist()
		{
			// Arrange
			var studentWork = SharedTestsData.TestStudents.FirstOrDefault()?.Works?.FirstOrDefault();
			await StudentWorkControllerSeeder.PrepareStudentWorksAsync(_fixture);

			// Act
			var response = await _client.GetAsync($"/api/studentwork/plagiarism/{studentWork?.Id}");

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var result = await response.Content.ReadFromJsonAsync<List<PlagiarismResponseDto>>();
			Assert.NotNull(result);
			Assert.Equal(3, result.Count);
			Assert.Equal(100, result.First().SimilarityPercentage);
			Assert.Equal("myWork.cs", result.First().Name);
			Assert.Equal("program.cs", result.Last().Name);
			Assert.True(result.Last().SimilarityPercentage < result.First().SimilarityPercentage);
		}

		[Fact]
		public async Task GetPlagiarismPercentages_ShouldReturnNotFound_WhenWorkNotExist()
		{
			// Arrange
			var studentWorkId = Guid.NewGuid();

			// Act
			var response = await _client.GetAsync($"/api/studentwork/plagiarism/{studentWorkId}");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}
	}
}