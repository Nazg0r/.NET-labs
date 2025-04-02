using DataAccess.Entities;
using Shared.DTOs;

namespace BusinessLogic.Mappers
{
	public static class EntityToDto
	{
		public static StudentWorkResponseDto ToDto(this StudentWork source) =>
			new StudentWorkResponseDto
			{
				Id = source.Id,
				Content = source.Content,
				FileName = source.FileName + source.Extension,
				LoadDate = source.LoadDate,
				StudentId = source.StudentId
			};
	}
}
