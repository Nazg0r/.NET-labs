using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using API.IntegrationTests.Seeders;
using FluentAssertions;
using Modules.Works.Application.Common.Models;
using Modules.Works.Domain.Entities;
using TestsTools;

namespace API.IntegrationTests
{
    public class WorksModuleTests(WebApplicationFixture fixture) : IClassFixture<WebApplicationFixture>
    {
        private readonly WebApplicationFixture _fixture = fixture;
        private readonly HttpClient _client = fixture.CreateClient();

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetWorkById_ShouldReturnOk_WithProcessedWorkResponse_WhenWorkExist(int index)
        {
            // Arrange
            Work? work = SharedTestsData.WorksWithDifferentExtension[index];

            await WorkSeeder.PrepareWorkAsync(_fixture, work);

            // Act
            var response = await _client.GetAsync($"/api/studentwork/{work?.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProcessedWorkResponse>();
            Assert.Equal(work?.Id, result?.Id);
            Assert.Equal(work?.Content, result?.Content);
            Assert.Equal(work?.FileName + work?.Extension, result?.FileName);
            Assert.Equal(work?.LoadDate, result?.LoadDate);
            Assert.Equal(work?.StudentId, result?.StudentId);
        }

        [Fact]
        public async Task GetWorkById_ShouldReturnNotFound_WhenWorkNotExist()
        {
            // Arrange
            var studentWorkId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/studentwork/{studentWorkId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllWorks_ShouldReturnOk_WithListOfProcessedWorkResponse_WhenWorksExist()
        {
            // Arrange
            List<Work> studentWorks = SharedTestsData.WorksWithSameExtension;
            List<ProcessedWorkResponse> expected = studentWorks.Select(w => new ProcessedWorkResponse
            {
                Id = w.Id,
                Content = w.Content,
                FileName = w.FileName + w.Extension,
                LoadDate = w.LoadDate,
                StudentId = w.StudentId
            }).ToList();

            await WorkSeeder.PrepareWorksAsync(_fixture, studentWorks);

            // Act
            var response = await _client.GetAsync("/api/studentwork");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<List<ProcessedWorkResponse>>();
            Assert.NotNull(result);
            expected.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAllStudentWorks_ShouldReturnNotFound_WhenWorksNotExist()
        {
            await WorkSeeder.RemoveWorksAsync(_fixture);
            // Act
            var response = await _client.GetAsync("/api/studentwork");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task StoreWork_ShouldReturnOk_WithProcessedWorkResponse_WhenWorkIsStored()
        {
            // Arrange
            var student = SharedTestsData.TestStudents.FirstOrDefault()!;
            var password = "qwertyui";
            var fileName = "test.txt";
            var fileContent = "Hello, World!";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var file = new StreamContent(fileStream);
            file.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var content = new MultipartFormDataContent
            {
                { file, "file", fileName }
            };
            await StudentSeeder.PrepareRegisteredStudentAsync(_fixture, student, password);

            // Act
            var response = await _client.PostAsync($"/api/studentwork/upload/{student?.Id}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response);
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
            var studentWork = SharedTestsData.WorksWithDifferentExtension[0];
            await WorkSeeder.PrepareWorkAsync(_fixture, studentWork);

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
            var works = SharedTestsData.WorksWithSameExtension.Select(w => new Work()
            {
                Id = Guid.NewGuid(),
                Content = w.Content,
                FileName = w.FileName,
                LoadDate = w.LoadDate,
                Extension = w.Extension,
                StudentId = w.StudentId
            }).ToList();

            await WorkSeeder.RemoveWorksAsync(_fixture);
            await WorkSeeder.PrepareWorksAsync(_fixture, works);

            // Act
            var response = await _client.GetAsync($"/api/studentwork/plagiarism/{works[0].Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<List<GetSimilarityPercentageResponse>>();
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal(100, result.First().SimilarityPercentage);
            Assert.Equal("copy.cs", result.First().Name);
            Assert.Equal("irrelevant.cs", result.Last().Name);
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