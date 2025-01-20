using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.ResetPassword;

public class ResetPasswordCommand : Command<ResetPasswordCommand>
{
    public ResetPasswordCommand(string password, string confirmPassword, string email, string token)
    {
        AggregateId = Guid.NewGuid();
        Password = password;
        ConfirmPassword = confirmPassword;
        Email = email;
        Token = token;
    }

    public string Password { get; private set; }
    public string ConfirmPassword { get; private set; }
    public string Email { get; private set; }
    public string Token { get; private set; }
}
