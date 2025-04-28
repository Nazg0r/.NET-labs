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

		public static StudentWorkDto ToStudentWorkDto(this StudentWork source) =>
			new StudentWorkDto
			{
				Id = source.Id,
				FileName = source.FileName + source.Extension,
				LoadDate = source.LoadDate,
				Content = source.Content
			};

		public static StudentResponseDto ToDto(this Student source) =>
			new StudentResponseDto
			{
				Id = source.Id,
				Username = source.UserName!,
				Name = source.Name,
				Surname = source.Surname,
				Group = source.Group,
				Works = source.Works?.Select(w => w.ToStudentWorkDto()).ToList()
			};
	}
}
