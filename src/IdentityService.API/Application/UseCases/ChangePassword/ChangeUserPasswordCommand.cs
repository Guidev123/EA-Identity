using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.ChangePassword;

public class ChangeUserPasswordCommand : Command<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommand(string email, string oldPassword,
                                     string newPassword, string confirmNewPassword)
    {
        AggregateId = Guid.NewGuid();
        Email = email;
        OldPassword = oldPassword;
        NewPassword = newPassword;
        ConfirmNewPassword = confirmNewPassword;
    }

    public string Email { get; private set; }
    public string OldPassword { get; private set; }
    public string NewPassword { get; private set; }
    public string ConfirmNewPassword { get; private set; }
}
