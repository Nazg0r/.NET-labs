namespace Shared.DTOs
{
	public class PlagiarismResponseDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public double SimilarityPercentage { get; set; } = default!;
	}
}
