using System.Text.Json.Serialization;

namespace Shared.DTOs
{
	public class StudentWorkDto
	{
		public Guid Id { get; set; } = default!;
		public string FileName { get; set; } = default!;
		public DateTime LoadDate { get; set; } = default!;
		//[JsonIgnore]
		public byte[]? Content { get; set; }
	}
}
