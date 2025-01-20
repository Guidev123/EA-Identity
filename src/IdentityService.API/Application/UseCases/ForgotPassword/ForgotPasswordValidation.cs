using FluentValidation;

namespace IdentityService.API.Application.UseCases.ForgotPassword;

public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.ClientUriToResetPassword)
            .NotEmpty()
            .WithMessage("ClientUriToResetPassword is required")
            .Must(x => x.Contains("http://") || x.Contains("https://"))
            .WithMessage("ClientUriToResetPassword must be a valid URL");
    }
}
