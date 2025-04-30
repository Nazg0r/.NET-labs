using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
	public class Student : IdentityUser
	{
		[Column("name")]
		public string Name { get; set; } = default!;
		[Column("surname")]
		public string Surname { get; set; } = default!;
		[Column("group")]
		public string Group { get; set; } = default!;
		[Column("works")]
		public virtual List<StudentWork>? Works { get; set; } = new();
	}
}
