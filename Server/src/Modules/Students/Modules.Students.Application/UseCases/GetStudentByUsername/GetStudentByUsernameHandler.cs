namespace Modules.Students.Application.UseCases.GetStudentByUsername
{
	public class GetStudentByUsernameHandler(IStudentQueries studentQueries)
		: IQueryHandler<string, GetStudentByUsernameResponse>
	{
		public async Task<GetStudentByUsernameResponse> HandleAsync(string query,
			CancellationToken cancellationToken)
		{
			var student = await studentQueries.GetStudentByUsername(query);
			return student.ToDto();
		}
	}
}