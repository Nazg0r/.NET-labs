using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Presentation.Controllers
{
	public class StudentController(IAuthentificationService authentificationService)
		: BaseController
	{
		[HttpPost("register")]
		public async Task<ActionResult> Register(RegistrationRequestDto request)
		{
			await authentificationService.RegisterAsync(request);

			return Created();
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login(LoginRequestDto request)
		{
			var response = await authentificationService.LoginAsync(request);

			return Ok(response);
		}
	}
}
