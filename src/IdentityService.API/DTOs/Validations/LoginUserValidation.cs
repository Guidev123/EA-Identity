using FluentValidation;

namespace IdentityService.API.DTOs.Validations
{
    public class LoginUserValidation : AbstractValidator<LoginUserDTO>
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
