namespace DataAccess.Entities
{
	public class StudentWork : Entity
	{
		[Column("file_name")]
		public string FileName { get; set; } = default!;
		[Column("load_date")]
		public DateTime LoadDate { get; set; }
		[Column("extension")]
		public string Extension { get; set; } = default!;
		[Column("content")]
		public byte[] Content { get; set; } = default!;
		[Column("student_id")]
		public Guid StudentId { get; set; }
		public Student Student { get; set; } = default!;
	}
}
