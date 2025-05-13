namespace Modules.Students.Domain.Entities
{
	public class Student
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Surname { get; set; } = null!;
		public string Group { get; set; } = null!;
		public string Username { get; set; } = null!;
		public List<Guid> WorksIds { get; set; } = [];
	}
}