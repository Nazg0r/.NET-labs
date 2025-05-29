using Microsoft.AspNetCore.Identity;
using Modules.Students.Domain.Entities;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Identity;
using Modules.Students.Infrastructure.Query;
using Moq;

namespace Infrastructure.Tests
{
    public class QueryTests
    {
        private readonly Mock<UserManager<StudentIdentity>> _mockUserManager;
        public QueryTests()
        {
            var userStoreMock = new Mock<IUserStore<StudentIdentity>>();
            _mockUserManager = new Mock<UserManager<StudentIdentity>>
            (
                userStoreMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
        }

        public class GetStudentByUsernameAsyncTests : QueryTests
        {
            [Fact]
            public async Task Should_ReturnStudent_WhenStudentExists()
            {
                // Arrange
                var username = "teststudentid";
                var studentIdentity = new StudentIdentity
                {
                    UserName = username,
                    Name = "Test",
                    Surname = "User",
                    Group = "TestGroup",
                    WorksIds = new List<Guid>()
                };

                _mockUserManager.Setup(x =>
                    x.FindByNameAsync(username)).ReturnsAsync(studentIdentity);

                var studentQueries = new StudentQueries(_mockUserManager.Object);

                // Act
                var result = await studentQueries.GetStudentByUsernameAsync(username);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<Student>(result);
                Assert.Equal(username, result.Username);
                _mockUserManager.Verify(x => x.FindByNameAsync(username), Times.Once);
            }

            [Fact]
            public async Task Should_ThrowStudentNotFoundException_WhenStudentNotExists()
            {
                // Arrange
                var username = "teststudentid";
                StudentIdentity? studentIdentity = null;

                _mockUserManager.Setup(x =>
                    x.FindByNameAsync(username)).ReturnsAsync(studentIdentity);

                var studentQueries = new StudentQueries(_mockUserManager.Object);

                // Act Assert
                await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
                {
                    await studentQueries.GetStudentByUsernameAsync(username);
                });
            }
        }

        public class GetStudentByWorkIdTests : QueryTests
        {
            [Fact]
            public void Should_ReturnStudent_WhenStudentExists()
            {
                // Arrange
                var workId = Guid.NewGuid();
                var studentIdentity = new StudentIdentity
                {
                    UserName = "teststudentid",
                    Name = "Test",
                    Surname = "User",
                    Group = "TestGroup",
                    WorksIds = new List<Guid> { workId }
                };

                _mockUserManager.Setup(x => x.Users)
                    .Returns(new List<StudentIdentity> { studentIdentity }.AsQueryable());

                var studentQueries = new StudentQueries(_mockUserManager.Object);

                // Act
                var result = studentQueries.GetStudentByWorkId(workId);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<Student>(result);
                Assert.Contains(workId, result.WorksIds);
                _mockUserManager.Verify(x => x.Users, Times.Once);
            }

            [Fact]
            public void Should_ThrowStudentNotFoundException_WhenStudentNotExists()
            {
                // Arrange
                var workId = Guid.NewGuid();
                var studentIdentity = new StudentIdentity
                {
                    UserName = "teststudentid",
                    Name = "Test",
                    Surname = "User",
                    Group = "TestGroup",
                    WorksIds = new List<Guid>()
                };
                _mockUserManager.Setup(x => x.Users)
                    .Returns(new List<StudentIdentity> { studentIdentity }.AsQueryable());

                var studentQueries = new StudentQueries(_mockUserManager.Object);
                // Act & Assert
                Assert.Throws<StudentNotFoundException>(() =>
                {
                    studentQueries.GetStudentByWorkId(workId);
                });
            }
        }

        public class GetStudentByIdTests : QueryTests
        {
            [Fact]
            public async Task Should_ReturnStudent_WhenStudentExists()
            {
                // Arrange
                var studentId = Guid.NewGuid().ToString();
                var studentIdentity = new StudentIdentity
                {
                    Id = studentId,
                    UserName = "testuser",
                    Name = "Test",
                    Surname = "User",
                    Group = "TestGroup",
                    WorksIds = new List<Guid>()
                };

                _mockUserManager.Setup(x =>
                    x.FindByIdAsync(studentId)).ReturnsAsync(studentIdentity);

                var studentQueries = new StudentQueries(_mockUserManager.Object);

                // Act
                var result = await studentQueries.GetStudentByIdAsync(studentId);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<Student>(result);
                Assert.Equal(studentId, result.Id.ToString());
                _mockUserManager.Verify(x => x.FindByIdAsync(studentId), Times.Once);
            }

            [Fact]
            public async Task Should_ThrowStudentNotFoundException_WhenStudentNotExists()
            {
                // Arrange
                var studentId = Guid.NewGuid().ToString();
                StudentIdentity? studentIdentity = null;

                _mockUserManager.Setup(x =>
                    x.FindByIdAsync(studentId)).ReturnsAsync(studentIdentity);

                var studentQueries = new StudentQueries(_mockUserManager.Object);

                // Act & Assert
                await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
                    await studentQueries.GetStudentByIdAsync(studentId));
            }
        }
    }
}