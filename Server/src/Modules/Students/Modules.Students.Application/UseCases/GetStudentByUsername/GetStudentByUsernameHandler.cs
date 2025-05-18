using BuildingBlocks.Contracts;
using MassTransit;
using Modules.Students.IntegrationEvents;
using Modules.Works.IntegrationEvents;

namespace Modules.Students.Application.UseCases.GetStudentByUsername
{
	public class GetStudentByUsernameHandler(
		IStudentQueries studentQueries,
		IRequestClient<GetStudentWorksRequest> client)
		: IQueryHandler<string, GetStudentByUsernameResponse>
	{
		public async Task<GetStudentByUsernameResponse> HandleAsync(string query,
			CancellationToken cancellationToken)
		{
			var student = await studentQueries.GetStudentByUsernameAsync(query);

			var studentWorks = await client.GetResponse<GetStudentWorksResponse>(
				new GetStudentWorksRequest(student.Id.ToString()), cancellationToken);

			var response = student.ToDto();
			response.Works = studentWorks.Message.Works;

			return response;
		}
	}
}