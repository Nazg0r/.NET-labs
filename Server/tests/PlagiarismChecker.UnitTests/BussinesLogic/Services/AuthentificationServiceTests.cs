using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.DTOs;
using Shared.Errors;

namespace PlagiarismChecker.BussinesLogic.Services
{
	public class AuthentificationServiceTests
	{
		private Mock<UserManager<Student>> _mockUserManager;
		private Mock<SignInManager<Student>> _mockSignInManager;
		private Mock<IConfiguration> _mockConfiguration;
		private AuthentificationService _service;
		AuthentificationServiceTests()
		{
			_mockUserManager = GetUserManagerMock();
			_mockSignInManager = GetSignInManagerMock(_mockUserManager.Object);
			_mockConfiguration = new Mock<IConfiguration>();
			_mockConfiguration.Setup(c => c["JWT:Secret"]).Returns("supersecretkeysupersecretkey123!");
			_mockConfiguration.Setup(c => c["JWT:Expires"]).Returns("1");
			_service = new AuthentificationService(
				_mockUserManager.Object,
				_mockSignInManager.Object,
				_mockConfiguration.Object
			);
		}

		public class RegisterAsync : AuthentificationServiceTests
		{
			[Fact]
			public async Task Should_RegisterStudent_WhenValidInput()
			{
				// Arrange
				var password = "qwerty";
				var credentials = new RegistrationRequestDto
				{
					Username = "johnDoe",
					Name = "John",
					Surname = "Doe",
					Group = "IM-00",
					Password = password
				};

				_mockUserManager.Setup
					(x => x.CreateAsync(It.IsAny<Student>(), password))
					.ReturnsAsync(IdentityResult.Success);

				// Act
				await _service.RegisterAsync(credentials);

				// Assert
				_mockUserManager.Verify(m => m.CreateAsync(It.IsAny<Student>(), password), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentCreationException_WhenUserCreationFails()
			{
				// Arrange
				var password = "qwerty";
				var credentials = new RegistrationRequestDto
				{
					Username = "johnDoe",
					Name = "John",
					Surname = "Doe",
					Group = "IM-00",
					Password = password
				};

				_mockUserManager.Setup
					(x => x.CreateAsync(It.IsAny<Student>(), password))
					.ReturnsAsync(IdentityResult.Failed());

				// Act & Assert
				await Assert.ThrowsAsync<StudentCreationException>(() => _service.RegisterAsync(credentials));
			}
		}

		public class LoginAsync : AuthentificationServiceTests
		{
			[Fact]
			public async Task Should_ReturnLoginResponseDto_WhenValidInput()
			{
				// Arrange
				var password = "qwerty";
				var credentials = new LoginRequestDto
				{
					Username = "johnDoe",
					Password = password
				};
				var student = new Student
				{
					UserName = credentials.Username,
					Name = "John",
					Surname = "Doe",
					Group = "IM-00"
				};

				_mockUserManager.Setup(x => x.FindByNameAsync(credentials.Username))
					.ReturnsAsync(student);
				_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(student, password, false))
					.ReturnsAsync(SignInResult.Success);

				// Act
				var result = await _service.LoginAsync(credentials);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<LoginResponseDto>(result);
				Assert.False(string.IsNullOrEmpty(result.Token));
				Assert.True(result.ExpiresDate > DateTime.Now);
				_mockUserManager.Verify(m => m.FindByNameAsync(credentials.Username), Times.Once);
				_mockSignInManager.Verify(m => m.CheckPasswordSignInAsync(student, password, false), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentNotFoundException_WhenUserNotExist()
			{
				// Arrange
				var credentials = new LoginRequestDto
				{
					Username = "johnDoe",
					Password = "qwerty"
				};
				var student = (Student)null!;

				_mockUserManager.Setup(x => x.FindByNameAsync(credentials.Username))
					.ReturnsAsync(student);

				// Act & Assert
				await Assert.ThrowsAsync<StudentNotFoundException>(() => _service.LoginAsync(credentials));
			}

			[Fact]
			public async Task Should_ThrowUnauthorizedException_WhenUserNotExist()
			{
				// Arrange
				var password = "qwerty";
				var credentials = new LoginRequestDto
				{
					Username = "johnDoe",
					Password = password
				};
				var student = new Student
				{
					UserName = credentials.Username,
					Name = "John",
					Surname = "Doe",
					Group = "IM-00"
				};

				_mockUserManager.Setup(x => x.FindByNameAsync(credentials.Username))
					.ReturnsAsync(student);
				_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(student, password, false))
					.ReturnsAsync(SignInResult.Failed);

				// Act & Assert
				await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(credentials));

			}
		}

		private Mock<UserManager<Student>> GetUserManagerMock()
		{
			var userStoreMock = new Mock<IUserStore<Student>>();
			var userManagerMock = new Mock<UserManager<Student>>
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
			return userManagerMock;
		}

		private Mock<SignInManager<Student>> GetSignInManagerMock(UserManager<Student> userManager)
		{
			var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
			var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<Student>>();
			var signInManagerMock = new Mock<SignInManager<Student>>(
				userManager,
				contextAccessor.Object,
				userPrincipalFactory.Object,
				null,
				null,
				null,
				null
			);
			return signInManagerMock;
		}
	}
}
