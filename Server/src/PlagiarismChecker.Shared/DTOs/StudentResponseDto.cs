using System.Text.Json.Serialization;

namespace Shared.DTOs
{
	public class StudentResponseDto
	{
		public string Id { get; set; } = default!;
		public string Username { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string Surname { get; set; } = default!;
		public string Group { get; set; } = default!;

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public List<StudentWorkDto>? Works { get; set; }
	}
}
