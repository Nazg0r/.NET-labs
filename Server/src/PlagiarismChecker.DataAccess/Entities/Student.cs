namespace DataAccess.Entities
{
	public class Student : Entity
	{
		public string Name { get; set; } = default!;
		public string Surname { get; set; } = default!;
		public string Group { get; set; } = default!;
		public List<StudentWork> Works { get; set; } = new();
	}
}
