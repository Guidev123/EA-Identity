using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.ChangePassword;

public class ChangeUserPasswordCommand(string email, string oldPassword, string newPassword, string confirmNewPassword)
           : Command<ChangeUserPasswordCommand>
{
    public string Email { get; private set; } = email;
    public string OldPassword { get; private set; } = oldPassword;
    public string NewPassword { get; private set; } = newPassword;
    public string ConfirmNewPassword { get; private set; } = confirmNewPassword;
}
