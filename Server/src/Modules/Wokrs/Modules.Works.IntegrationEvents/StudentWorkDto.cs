namespace Modules.Works.IntegrationEvents
{
	public record StudentWorkDto
	{
		public Guid Id { get; init; } = Guid.Empty!;
		public string FileName { get; init; } = null!;
		public DateTime LoadDate { get; init; } = default!;
		public byte[]? Content { get; init; }
	}
}
