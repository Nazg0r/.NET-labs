using BuildingBlocks.Contracts;

namespace Modules.Students.Application.UseCases.Register
{
	public class RegisterStudentHandler(IStudentCreator studentCreator) : ICommandHandler<RegisterStudentCommand>
	{
		public async Task HandleAsync(RegisterStudentCommand command, CancellationToken token = default)
		{
			var student = command.ToEntity();
			await studentCreator.CreateAsync(student, command.Password, token);
		}
	}
}
