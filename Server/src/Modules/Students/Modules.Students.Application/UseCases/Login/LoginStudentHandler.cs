using BuildingBlocks.Contracts;

namespace Modules.Students.Application.UseCases.Login
{
    public class LoginStudentHandler(
        IJwtTokenGenerator tokenGenerator,
        IStudentAuthenticator studentAuthenticator)
        : ICommandHandler<LoginStudentCommand, LoginStudentResponse>
    {
        public async Task<LoginStudentResponse> HandleAsync(LoginStudentCommand command,
            CancellationToken cancellationToken)
        {
            var student =
                await studentAuthenticator.ValidateCredentialsAsync(command.Username, command.Password);

            var token = tokenGenerator.GenerateToken(student);

            return new LoginStudentResponse()
            {
                Token = token,
                ExpiresDate = tokenGenerator.GetTokenExpiry()
            };
        }
    }
}