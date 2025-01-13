using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Register;

public class RegisterUserCommand(string Name, string Cpf,
                                  string Email, string Password,
                                  string ConfirmPassword) : Command<LoginResponseDTO>
{
    public string Name { get; private set; } = Name;
    public string Cpf { get; private set; } = Cpf;
    public string Email { get; private set; } = Email;
    public string Password { get; private set; } = Password;
    public string ConfirmPassword { get; private set; } = ConfirmPassword;
}
