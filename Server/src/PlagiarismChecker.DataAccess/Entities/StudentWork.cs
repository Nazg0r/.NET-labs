namespace DataAccess.Entities
{
	public class StudentWork : Entity
	{
		public string FileName { get; set; } = default!;
		public DateTime LoadDate { get; set; }
		public string Extension { get; set; } = default!;
		public byte[] Content { get; set; } = default!;

	}
}
