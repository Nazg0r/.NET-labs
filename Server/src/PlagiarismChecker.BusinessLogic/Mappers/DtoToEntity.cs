using DataAccess.Entities;
using Shared.DTOs;

namespace BusinessLogic.Mappers
{
	public static class DtoToEntity
	{
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
