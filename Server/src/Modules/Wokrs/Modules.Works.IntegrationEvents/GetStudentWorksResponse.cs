namespace Modules.Works.IntegrationEvents
{
	public record GetStudentWorksResponse
	{
		public List<StudentWorkDto> Works { get; init; } = [];
	}
}
