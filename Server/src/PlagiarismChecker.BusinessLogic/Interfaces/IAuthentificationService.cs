using Shared.DTOs;

namespace BusinessLogic.Interfaces
{
	public interface IAuthentificationService
	{
		public Task RegisterAsync(RegistrationRequestDto credentials);
		public Task<LoginResponseDto> LoginAsync(LoginRequestDto credentials);

	}
}
