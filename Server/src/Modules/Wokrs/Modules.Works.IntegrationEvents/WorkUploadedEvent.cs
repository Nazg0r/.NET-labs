namespace Modules.Works.IntegrationEvents
{
	public record WorkUploadedEvent(Guid workId, string studentId);
}
