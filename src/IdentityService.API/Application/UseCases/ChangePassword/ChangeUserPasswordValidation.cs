using FluentValidation;

namespace IdentityService.API.Application.UseCases.ChangePassword
{
    public class ChangeUserPasswordValidation : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("The Email field is required")
                .EmailAddress().WithMessage("The Email field is in an invalid format");

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("The OldPassword field is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("The NewPassword field is required")
                .Length(6, 100).WithMessage("The NewPassword field must be between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("The new passwords do not match")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));
        }
    }
}