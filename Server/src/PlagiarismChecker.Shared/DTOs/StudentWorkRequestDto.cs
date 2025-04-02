namespace Shared.DTOs
{
	public class StudentWorkRequestDto
	{
		public string FileName { get; set; } = default!;
		public string Extension { get; set; } = default!;
		public byte[] Content { get; set; } = default!;
		public string StudentId { get; set; } = default!;
	}
}
