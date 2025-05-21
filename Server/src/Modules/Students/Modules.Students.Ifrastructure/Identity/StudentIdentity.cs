using Microsoft.AspNetCore.Identity;

namespace Modules.Students.Infrastructure.Identity
{
	public class StudentIdentity : IdentityUser
	{
		public string Name { get; set; } = null!;
		public string Surname { get; set; } = null!;
		public string Group { get; set; } = null!;
		public List<Guid> WorksIds { get; set; } = [];
	}
}
