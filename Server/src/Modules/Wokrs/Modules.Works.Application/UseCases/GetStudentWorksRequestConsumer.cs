using MassTransit;
using Modules.Students.IntegrationEvents;
using Modules.Works.Application.Common.Mappings;
using Modules.Works.IntegrationEvents;

namespace Modules.Works.Application.UseCases
{
	public class GetStudentWorksRequestConsumer(IWorkRepository repository)
		: IConsumer<GetStudentWorksRequest>
	{
		public async Task Consume(ConsumeContext<GetStudentWorksRequest> context)
		{
			var studentWorks =
				await repository.GetWorksByStudentIdAsync(context.Message.id);

			var response = new GetStudentWorksResponse
			{
				Works = studentWorks.Select(w => w.ToEventResponse()).ToList()
			};

			await context.RespondAsync<GetStudentWorksResponse>(response);
		}
	}
}