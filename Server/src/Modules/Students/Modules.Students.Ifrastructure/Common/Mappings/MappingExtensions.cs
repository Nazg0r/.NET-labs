using Modules.Students.Domain.Entities;
using Modules.Students.Infrastructure.Identity;

namespace Modules.Students.Infrastructure.Common.Mappings
{
    public static class MappingExtensions
    {
        public static StudentIdentity ToIdentity(this Student source)
        {
            return new StudentIdentity
            {
                UserName = source.Username,
                Name = source.Name,
                Surname = source.Surname,
                Group = source.Group,
                WorksIds = source.WorksIds
            };
        }

        public static Student ToDomain(this StudentIdentity source)
        {
            return new Student
            {
                Id = Guid.Parse(source.Id),
                Username = source.UserName!,
                Name = source.Name,
                Surname = source.Surname,
                Group = source.Group,
                WorksIds = source.WorksIds
            };
        }
    }
}