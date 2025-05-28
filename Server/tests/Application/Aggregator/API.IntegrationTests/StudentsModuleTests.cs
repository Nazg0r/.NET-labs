using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using API.IntegrationTests.Seeders;
using Modules.Students.Application.Common.Models;
using Modules.Students.Domain.Entities;
using Modules.Works.Domain.Entities;
using TestsTools;

namespace API.IntegrationTests;

public class StudentsModuleTests(WebApplicationFixture fixture) : IClassFixture<WebApplicationFixture>
{
    private readonly WebApplicationFixture _fixture = fixture;
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GetStudentByUsername_ShouldReturnOk_WithGetStudentByUsernameResponse_WhenUserExist()
    {
        {
            // Arrange
            var username = "johnDoe";
            Student student = new Student
            {
                Username = username,
                Name = "John",
                Surname = "Doe",
                Group = "IM-00"
            };
            await StudentSeeder.PrepareRegisteredStudentAsync(_fixture, student, "qwertyui");

            // Act
            var response = await _client.GetAsync($"/api/student/{username}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GetStudentByUsernameResponse>();
            Assert.NotNull(result);
            Assert.Equal(student.Username, result.Username);
            Assert.Equal(student.Name, result.Name);
            Assert.Equal(student.Surname, result.Surname);
            Assert.Equal(student.Group, result.Group);
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
        // Arrange

        Guid workId = Guid.NewGuid();
        Student student = new Student
        {
            Username = "testOne",
            Name = "Test",
            Surname = "Test",
            Group = "IM-11",
        };

        var work = new Work()
        {
            Id = workId,
            Content = Encoding.UTF8.GetBytes("hello world"),
            FileName = "test",
            LoadDate = DateTime.UtcNow,
            Extension = ".cs"
        };


        student = await StudentSeeder.PrepareRegisteredStudentAsync(_fixture, student, "qwertyui");
        work.StudentId = student.Id.ToString();
        await WorkSeeder.PrepareWorkAsync(_fixture, work, student);
        await StudentSeeder.AddWorkIdToStudentAsync(_fixture, student, workId);

        // Act
        var response = await _client.GetAsync($"/api/student/work/{workId}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<string>(json);
        Assert.NotNull(result);
        Assert.Equal($"{student.Name} {student.Surname} {student.Group}", result);
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
        Assert.Contains($"Student with workId: {workId} not found",
            await response.Content.ReadAsStringAsync());
    }
}