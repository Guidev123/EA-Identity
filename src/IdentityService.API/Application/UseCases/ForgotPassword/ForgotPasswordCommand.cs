using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.ForgotPassword;

public class ForgotPasswordCommand : Command<ForgotPasswordCommand>
{
    public ForgotPasswordCommand(string email, string clientUriToResetPassword)
    {
        AggregateId = Guid.NewGuid();
        Email = email;
        ClientUriToResetPassword = clientUriToResetPassword;
    }

    public string Email { get; private set; }
    public string ClientUriToResetPassword { get; private set; }
}
