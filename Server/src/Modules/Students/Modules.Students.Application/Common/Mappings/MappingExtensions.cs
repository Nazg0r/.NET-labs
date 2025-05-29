namespace Modules.Students.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        public static Student ToEntity(this RegisterStudentCommand source) =>
            new Student
            {
                Username = source.Username,
                Name = source.Name,
                Surname = source.Surname,
                Group = source.Group,
            };

        public static GetStudentByUsernameResponse ToDto(this Student source) =>
            new GetStudentByUsernameResponse
            {
                Id = source.Id.ToString(),
                Username = source.Username!,
                Name = source.Name,
                Surname = source.Surname,
                Group = source.Group,
            };
    }
}