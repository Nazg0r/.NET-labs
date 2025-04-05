using FluentValidation;
using Shared.DTOs;

namespace Presentation.Validators
{
	public class RegistrationRequestDtoValidator : AbstractValidator<RegistrationRequestDto>
	{
		public RegistrationRequestDtoValidator() 
		{
			RuleFor(x => x.Username)
				.NotEmpty()
				.WithMessage("Username is required")
				.MinimumLength(6)
				.WithMessage("Username must be at least 6 character long");

			RuleFor(x => x.Name)
				.NotEmpty()
				.WithMessage("Name is required");

			RuleFor(x => x.Surname)
				.NotEmpty()
				.WithMessage("Surname is required");

			RuleFor(x => x.Group)
				.NotEmpty()
				.WithMessage("Group is required")
				.Length(5,6)
				.WithMessage("Incorect group format");

			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage("Password is required")
				.MinimumLength(8)
				.WithMessage("Password must be at least 8 character long");
		}
	}
}
