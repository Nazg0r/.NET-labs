using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Modules.Students.Domain.Entities;
using Modules.Students.Domain.Exceptions;
using Modules.Students.Infrastructure.Authentification;
using Modules.Students.Infrastructure.Identity;
using Moq;
using TestsTools;

namespace Infrastructure.Tests;

public class AuthentificationTests
{
	private readonly Mock<UserManager<StudentIdentity>> _mockUserManager;
	private readonly Mock<SignInManager<StudentIdentity>> _mockSignInManager;
	private readonly Mock<IConfiguration> _mockConfiguration;

	public AuthentificationTests()
	{
		_mockUserManager = GetUserManagerMock();
		_mockSignInManager = GetSignInManagerMock(_mockUserManager.Object);
		_mockConfiguration = new Mock<IConfiguration>();
		_mockConfiguration.Setup(c => c["JWT:Secret"]).Returns("supersecretkeysupersecretkey123!");
		_mockConfiguration.Setup(c => c["JWT:Expires"]).Returns("1");
	}

	public class StudentAuthenticatorTests : AuthentificationTests
	{
		[Fact]
		public async Task Should_ReturnStudent_WhenCredentialsValid()
		{
			// Arrange
			string username = "testuser";
			string password = "testpassword";
			var identity = new StudentIdentity
			{
				UserName = username,
				Name = "Test",
				Surname = "User",
				Group = "TestGroup",
				WorksIds = new List<Guid>()
			};

			_mockUserManager.Setup(x => x.FindByNameAsync(username))
				.ReturnsAsync(identity);

			_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(identity, password, false))
				.ReturnsAsync(SignInResult.Success);

			var authentificator = new IdentityStudentAuthenticator(_mockUserManager.Object, _mockSignInManager.Object);

			// Act
			var result = await authentificator.ValidateCredentialsAsync(username, password);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<Student>(result);
			Assert.Equal(username, result.Username);
			Assert.Equal(identity.Name, result.Name);
			Assert.Equal(identity.Surname, result.Surname);
			Assert.Equal(identity.Group, result.Group);
			Assert.Equal(identity.WorksIds, result.WorksIds);
			_mockUserManager.Verify(x => x.FindByNameAsync(username), Times.Once);
			_mockSignInManager.Verify(x =>
				x.CheckPasswordSignInAsync(identity, password, false), Times.Once);
		}

		[Fact]
		public async Task Should_ThrowStudentNotFoundException_WhenUserNotExist()
		{
			// Arrange
			string username = "testuser";
			string password = "testpassword";
			StudentIdentity? identity = null;

			_mockUserManager.Setup(x => x.FindByNameAsync(username))
				.ReturnsAsync(identity);

			var authentificator = new IdentityStudentAuthenticator(_mockUserManager.Object, _mockSignInManager.Object);

			// Act & Assert
			await Assert.ThrowsAsync<StudentNotFoundException>(() =>
				authentificator.ValidateCredentialsAsync(username, password));
		}

		[Fact]
		public async Task Should_ThrowUnauthorizedException_PasswordCheckingFaild()
		{
			// Arrange
			string username = "testuser";
			string password = "testpassword";
			var identity = new StudentIdentity
			{
				UserName = username,
				Name = "Test",
				Surname = "User",
				Group = "TestGroup",
				WorksIds = new List<Guid>()
			};

			_mockUserManager.Setup(x => x.FindByNameAsync(username))
				.ReturnsAsync(identity);

			_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(identity, password, false))
				.ReturnsAsync(SignInResult.Failed);

			var authentificator = new IdentityStudentAuthenticator(_mockUserManager.Object, _mockSignInManager.Object);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedException>(() =>
				authentificator.ValidateCredentialsAsync(username, password));
		}

		public class JwtTokenGeneratorTests : AuthentificationTests
		{
			[Fact]
			public void Should_GenerateToken_WhenStudentIsValid()
			{
				// Arrange
				var student = new Student
				{
					Username = "testuser",
					Name = "Test",
					Surname = "User",
					Group = "TestGroup",
					WorksIds = new List<Guid>()
				};
				var jwtTokenGenerator = new JwtTokenGenerator(_mockConfiguration.Object);

				// Act
				var token = jwtTokenGenerator.GenerateToken(student);

				// Assert
				Assert.NotNull(token);
				Assert.IsType<string>(token);
			}
		}

		public class StudentCreatorTests : AuthentificationTests
		{
			[Fact]
			public async Task Should_CreateStudent_WhenValidInput()
			{
				// Arrange
				var student = SharedTestsData.TestStudents.First();
				var password = "testpassword";
				var identity = new StudentIdentity
				{
					UserName = student.Username,
					Name = student.Name,
					Surname = student.Surname,
					Group = student.Group,
					WorksIds = student.WorksIds
				};

				_mockUserManager.Setup(x => x.CreateAsync(It.IsAny<StudentIdentity>(), password))
					.ReturnsAsync(IdentityResult.Success);

				var creator = new StudentCreator(_mockUserManager.Object);
				// Act

				await creator.CreateAsync(student, password);

				// Assert
				_mockUserManager.Verify(x => x.CreateAsync(It.Is<StudentIdentity>(s =>
						s.UserName == student.Username &&
						s.Name == student.Name &&
						s.Surname == student.Surname &&
						s.Group == student.Group &&
						s.WorksIds.SequenceEqual(student.WorksIds)),
					password), Times.Once);
			}

			[Fact]
			public async Task Should_ThrowStudentCreationException_WhenStudentCreationFailed()
			{
				// Arrange
				var student = SharedTestsData.TestStudents.First();
				var password = "testpassword";

				_mockUserManager.Setup(x => x.CreateAsync(It.IsAny<StudentIdentity>(), password))
					.ReturnsAsync(IdentityResult.Failed());

				var creator = new StudentCreator(_mockUserManager.Object);

				// Act & Assert
				await Assert.ThrowsAsync<StudentCreationException>(async () =>
					await creator.CreateAsync(student, password));
			}
		}
	}

	private Mock<UserManager<StudentIdentity>> GetUserManagerMock()
	{
		var userStoreMock = new Mock<IUserStore<StudentIdentity>>();
		var userManagerMock = new Mock<UserManager<StudentIdentity>>
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

	private Mock<SignInManager<StudentIdentity>> GetSignInManagerMock(UserManager<StudentIdentity> userManager)
	{
		var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
		var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<StudentIdentity>>();
		var signInManagerMock = new Mock<SignInManager<StudentIdentity>>(
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