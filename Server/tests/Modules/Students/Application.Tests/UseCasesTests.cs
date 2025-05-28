using MassTransit;
using Modules.Students.Application.Common.Models;
using Modules.Students.Application.Contracts;
using Modules.Students.Application.UseCases.GetStudentByUsername;
using Modules.Students.Application.UseCases.GetStudentByWorkId;
using Modules.Students.Application.UseCases.Login;
using Modules.Students.Application.UseCases.Register;
using Modules.Students.Domain.Entities;
using Modules.Students.IntegrationEvents;
using Modules.Works.IntegrationEvents;
using Moq;
using TestsTools;

namespace Application.Tests;

public class UseCasesTests
{
    public class Register : UseCasesTests
    {
        private readonly Mock<IStudentCreator> _mockStudentCreator;

        public Register()
        {
            _mockStudentCreator = new Mock<IStudentCreator>();
        }

        [Fact]
        public async Task Should_RegisterStudent_WhenValidInput()
        {
            // Arrange
            var password = "qwerty";
            var command = new RegisterStudentCommand
            {
                Username = "johnDoe",
                Name = "John",
                Surname = "Doe",
                Group = "IM-00",
                Password = password
            };

            var handler = new RegisterStudentHandler(_mockStudentCreator.Object);

            // Act
            await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            _mockStudentCreator.Verify(m =>
                    m.CreateAsync(It.Is<Student>(s =>
                            s.Username == command.Username &&
                            s.Name == command.Name &&
                            s.Surname == command.Surname &&
                            s.Group == command.Group),
                        password,
                        CancellationToken.None),
                Times.Once);
        }
    }

    public class Login : UseCasesTests
    {
        private readonly Mock<IStudentAuthenticator> _mockStudentAuthenticator;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

        public Login()
        {
            _mockStudentAuthenticator = new Mock<IStudentAuthenticator>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        }


        [Fact]
        public async Task Should_ReturnLoginStudentResponse_WhenValidInput()
        {
            // Arrange
            var password = "qwerty";
            var command = new LoginStudentCommand
            {
                Username = "johnDoe",
                Password = password
            };
            var student = new Student
            {
                Username = command.Username,
                Name = "John",
                Surname = "Doe",
                Group = "IM-00"
            };
            var token = Guid.NewGuid().ToString();

            _mockStudentAuthenticator.Setup(x =>
                x.ValidateCredentialsAsync(command.Username, command.Password)).ReturnsAsync(student);

            _mockJwtTokenGenerator.Setup(x =>
                x.GenerateToken(student)).Returns(token);

            _mockJwtTokenGenerator.Setup(x =>
                x.GetTokenExpiry()).Returns(DateTime.Now.AddDays(7));

            var handler = new LoginStudentHandler(
                _mockJwtTokenGenerator.Object,
                _mockStudentAuthenticator.Object);

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoginStudentResponse>(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
            Assert.Equal(token, result.Token);
            Assert.True(result.ExpiresDate > DateTime.Now);
            _mockStudentAuthenticator.Verify(x =>
                x.ValidateCredentialsAsync(command.Username, command.Password), Times.Once);
            _mockJwtTokenGenerator.Verify(x => x.GenerateToken(student), Times.Once);
            _mockJwtTokenGenerator.Verify(x => x.GetTokenExpiry(), Times.Once);
        }
    }

    public class GetStudentByUsername : UseCasesTests
    {
        private readonly Mock<IStudentQueries> _mockStudentQueries;
        private readonly Mock<IRequestClient<GetStudentWorksRequest>> _mockClient;
        private readonly Mock<Response<GetStudentWorksResponse>> _mockResponse;

        public GetStudentByUsername()
        {
            _mockStudentQueries = new Mock<IStudentQueries>();
            _mockClient = new Mock<IRequestClient<GetStudentWorksRequest>>();
            _mockResponse = new Mock<Response<GetStudentWorksResponse>>();
        }

        [Fact]
        public async Task Should_ReturnGetStudentByUsernameResponse_WhenUserExist()
        {
            // Arrange
            var student = SharedTestsData.TestStudents.First();
            var query = student.Username;
            var response = SharedTestsData.TestStudentsWorks.First();

            _mockStudentQueries.Setup(x =>
                x.GetStudentByUsernameAsync(query)).ReturnsAsync(student);

            _mockResponse.Setup(r => r.Message).Returns(response);

            _mockClient
                .Setup(x => x.GetResponse<GetStudentWorksResponse>(
                    It.IsAny<GetStudentWorksRequest>(),
                    It.IsAny<CancellationToken>(),
                    default(RequestTimeout))).ReturnsAsync(_mockResponse.Object);


            var handler = new GetStudentByUsernameHandler(_mockStudentQueries.Object, _mockClient.Object);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetStudentByUsernameResponse>(result);
            Assert.Equal(student.Id.ToString(), result.Id);
            Assert.Equal(student.Username, result.Username);
            Assert.Equal(student.Name, result.Name);
            Assert.Equal(student.Surname, result.Surname);
            Assert.Equal(student.Group, result.Group);
            Assert.Equal(response.Works, result.Works);
            _mockStudentQueries.Verify(x =>
                x.GetStudentByUsernameAsync(query), Times.Once);
            _mockResponse.Verify(r => r.Message, Times.Once);
            _mockClient.Verify(x => x.GetResponse<GetStudentWorksResponse>(
                It.IsAny<GetStudentWorksRequest>(),
                It.IsAny<CancellationToken>(),
                default(RequestTimeout)), Times.Once);

        }
    }

    public class GetStudentByWorkId : UseCasesTests
    {
        private readonly Mock<IStudentQueries> _mockStudentQueries;

        public GetStudentByWorkId()
        {
            _mockStudentQueries = new Mock<IStudentQueries>();
        }

        [Fact]
        public async Task Should_ReturnAuthor_WhenUserExist()
        {
            // Arrange
            var student = SharedTestsData.TestStudents.First();
            var work = SharedTestsData.TestStudentsWorks.First().Works.First();

            _mockStudentQueries.Setup(x =>
                x.GetStudentByWorkId(work.Id)).Returns(student);

            var handler = new GetAuthorByWorkIdHandler(_mockStudentQueries.Object);

            // Act
            var result = await handler.HandleAsync(work.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.Equal($"{student.Name} {student.Surname} {student.Group}", result);
            _mockStudentQueries.Verify(x => x.GetStudentByWorkId(work.Id), Times.Once);
        }
    }
}