using FluentValidation;
using IdentityService.API.DTOs;

namespace IdentityService.API.Application.UseCases.Login
{
    public class LoginUserValidation : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("The Email field is required")
                .EmailAddress().WithMessage("The Email field is in an invalid format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The Password field is required")
                .Length(6, 100).WithMessage("The Password field must be between {MinLength} and {MaxLength} characters");
        }
    }
}
