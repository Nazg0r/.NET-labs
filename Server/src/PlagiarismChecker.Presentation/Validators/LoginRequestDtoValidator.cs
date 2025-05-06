using FluentValidation;
using Shared.DTOs;

namespace Presentation.Validators
{
	public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
	{
		public LoginRequestDtoValidator()
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
