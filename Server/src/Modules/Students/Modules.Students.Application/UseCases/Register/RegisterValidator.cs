using FluentValidation;

namespace Modules.Students.Application.UseCases.Register
{
    public class RegisterValidator : AbstractValidator<RegisterStudentCommand>
    {
        public RegisterValidator()
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
                .Length(5, 6)
                .WithMessage("Incorrect group format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 character long");
        }
    }
}