using DataAccess.Entities;
using Shared.DTOs;

namespace BusinessLogic.Mappers
{
	public static class DtoToEntity
	{
		public static StudentWork ToEntity(this StudentWorkRequestDto source) =>
			new StudentWork
			{
				FileName = source.FileName,
				Extension = source.Extension,
				Content = source.Content,
				StudentId = source.StudentId,
				LoadDate = DateTime.UtcNow,
			};

		public static Student ToEntity(this RegistrationRequestDto source) =>
			new Student
			{ 
				UserName = source.Username,
				Name = source.Name,
				Surname = source.Surname,
				Group = source.Group,
			};

	}
}
