using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Login;

public class LoginUserCommand : Command<LoginResponseDTO>
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public LoginUserCommand(string email, string password)
    {
        AggregateId = Guid.NewGuid();
        Email = email;
        Password = password;
    }
}
