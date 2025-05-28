using Modules.Works.IntegrationEvents;

namespace Modules.Students.Application.Common.Models
{
    public class GetStudentByUsernameResponse
    {
        public string Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string Group { get; set; } = default!;

        public List<StudentWorkDto> Works { get; set; } = new List<StudentWorkDto>();
    }
}