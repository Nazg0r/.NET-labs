using FluentValidation;

namespace Modules.Students.Application.UseCases.Login
{
	public class LoginValidator : AbstractValidator<LoginStudentCommand>
	{
		public LoginValidator()
		{
			RuleFor(x => x.Username)
				.NotEmpty()
				.WithMessage("Username is required");

			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage("Password is required")
				.MinimumLength(8)
				.WithMessage("Password must be at least 8 character long");
		}
	}
}
