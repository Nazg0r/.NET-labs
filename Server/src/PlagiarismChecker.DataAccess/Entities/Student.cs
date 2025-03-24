namespace DataAccess.Entities
{
	public class Student : Entity
	{
		[Column("name")]
		public string Name { get; set; } = default!;
		[Column("surname")]
		public string Surname { get; set; } = default!;
		[Column("group")]
		public string Group { get; set; } = default!;
		[Column("works")]
		public List<StudentWork>? Works { get; set; } = new();
	}
}
