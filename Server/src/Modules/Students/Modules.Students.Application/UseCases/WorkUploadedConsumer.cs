using MassTransit;
using Modules.Works.IntegrationEvents;

namespace Modules.Students.Application.UseCases
{
	public class WorkUploadedConsumer(IStudentCommands studentCommands)
		: IConsumer<WorkUploadedEvent>
	{
		public async Task Consume(ConsumeContext<WorkUploadedEvent> context) =>
			await studentCommands.AddWorkIdToStudent(context.Message.studentId, context.Message.workId);
	}
}