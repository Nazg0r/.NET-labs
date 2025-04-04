using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Presentation.Controllers
{
	public class StudentWorkController(IStudentWorkService studentWorkService)
		: BaseController
	{
		[HttpGet("{id}")]
		public async Task<ActionResult<StudentWorkResponseDto>> GetWork(Guid id)
		{
			var response = await studentWorkService.GetWorkAsync(id);

			return Ok(response);
		}

		[HttpGet]
		public async Task<ActionResult> GetAllStudentWorks()
		{
			var response = await studentWorkService.GetWorksAsync();

			return Ok(response);
		}

		[HttpPost("upload/{id}")]
		public async Task<ActionResult<StudentWorkResponseDto>> StoreWork([FromForm] IFormFile file, string id)
		{
			var response = await studentWorkService.StoreWorkAsync(file, id);

			return Created($"/api/studentwork/{response.Id}", response);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteWork(Guid id)
		{
			await studentWorkService.DeleteWorkAsync(id);

			return NoContent();
		}
	}
}
