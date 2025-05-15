using Modules.Works.IntegrationEvents;

namespace Modules.Works.Application.Common.Mappings
{
	public static class MappingsExtensions
	{
		public static ProcessedWorkResponse ToDto(this Work source) =>
			new ProcessedWorkResponse
			{
				Id = source.Id,
				Content = source.Content,
				FileName = source.FileName + source.Extension,
				LoadDate = source.LoadDate,
				StudentId = source.StudentId
			};

		public static StudentWorkDto ToEventResponse(this Work source) =>
			new StudentWorkDto
			{
				Id = source.Id,
				FileName = source.FileName + source.Extension,
				LoadDate = source.LoadDate,
				Content = source.Content
			};
	}
}