using BusinessLogic.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Presentation.Controllers
{
	public class StudentController(
		IAuthentificationService authentificationService,
		IStudentService studentService
		)
		: BaseController
	{
		[HttpPost("register")]
		public async Task<ActionResult> Register(
			RegistrationRequestDto request, 
			IValidator<RegistrationRequestDto> validator)
		{
			validator.ValidateAndThrow(request);

			await authentificationService.RegisterAsync(request);

			return Created();
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login(
			LoginRequestDto request, 
			IValidator<LoginRequestDto> validator)
		{
			validator.ValidateAndThrow(request);

			var response = await authentificationService.LoginAsync(request);

			return Ok(response);
		}

		[HttpGet("{username}")]
		public async Task<ActionResult> GetStudentByUsername(string username)
		{
			var response = await studentService.GetStudentByUsernameAsync(username);

			return Ok(response);
		}

		[HttpGet("work/{id}")]
		public async Task<ActionResult> GetAuthorByWorkId(Guid id)
		{
			var response = await studentService.GetAuthorByWorkIdAsync(id);

			return Ok(response);
		}
	}
}
