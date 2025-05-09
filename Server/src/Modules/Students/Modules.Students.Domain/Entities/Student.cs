using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Modules.Students.Domain.Entities
{
	internal class Student : IdentityUser
	{
		[Column("name")] public string Name { get; set; } = default!;
		[Column("surname")] public string Surname { get; set; } = default!;
		[Column("group")] public string Group { get; set; } = default!;
	}
}