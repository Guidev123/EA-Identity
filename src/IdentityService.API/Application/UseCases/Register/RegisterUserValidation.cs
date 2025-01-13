using FluentValidation;

namespace IdentityService.API.Application.UseCases.Register
{
    public class RegisterUserValidation : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The Name field is required");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("The Cpf field is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("The Email field is required")
                .EmailAddress().WithMessage("The Email field is in an invalid format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The Password field is required")
                .Length(6, 100).WithMessage("The Password field must be between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("The passwords do not match")
                .When(x => !string.IsNullOrEmpty(x.Password));
        }
    }
}
