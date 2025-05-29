using Modules.Works.Application.Common.Mappings;
using Modules.Works.Application.Common.Models;
using Modules.Works.IntegrationEvents;
using TestsTools;

namespace Application.Tests;

public class MappingsTests
{
    [Fact]
    public void Should_ReturnProcessedWorkResponse_WhenCalledFromWork()
    {
        // Arrange
        var work = SharedTestsData.WorksWithDifferentExtension.First();

        // Act
        var result = work.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ProcessedWorkResponse>(result);
        Assert.Equal(work.Id, result.Id);
        Assert.Equal(work.FileName + work.Extension, result.FileName);
        Assert.Equal(work.LoadDate, result.LoadDate);
        Assert.Equal(work.Content, result.Content);
        Assert.Equal(work.StudentId, result.StudentId);
    }

    [Fact]
    public void Should_ReturnStudentWorkDto_WhenCalledFromWork()
    {
        // Arrange
        var work = SharedTestsData.WorksWithDifferentExtension.First();

        // Act
        var result = work.ToEventResponse();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StudentWorkDto>(result);
        Assert.Equal(work.Id, result.Id);
        Assert.Equal(work.FileName + work.Extension, result.FileName);
        Assert.Equal(work.LoadDate, result.LoadDate);
        Assert.Equal(work.Content, result.Content);
    }
}