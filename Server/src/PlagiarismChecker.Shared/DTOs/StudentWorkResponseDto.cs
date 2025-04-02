namespace Shared.DTOs
{
	public class StudentWorkResponseDto
	{
		public Guid Id { get; set; }
		public string FileName { get; set; } = default!;
		public DateTime LoadDate { get; set; }
		public byte[] Content { get; set; } = default!;	
		public string StudentId { get; set; } = default!;
	}
}
