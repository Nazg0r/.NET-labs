namespace Modules.Students.Application.UseCases.GetStudentByWorkId
{
	public class GetAuthorByWorkIdHandler(IStudentQueries studentQueries)
		: IQueryHandler<Guid, string>
	{
		public async Task<string> HandleAsync(Guid query, CancellationToken cancellationToken)
		{
			var student = studentQueries.GetStudentByWorkId(query);
			return $"{student.Name} {student.Surname} {student.Group}";
		}
	}
}