using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Login;

public class LoginUserCommand(string email, string password) : Command<LoginResponseDTO>
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
}
