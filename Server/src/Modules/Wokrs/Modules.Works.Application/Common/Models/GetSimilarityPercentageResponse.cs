namespace Modules.Works.Application.Common.Models
{
	public class GetSimilarityPercentageResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public double SimilarityPercentage { get; set; } = 0!;
	}
}
