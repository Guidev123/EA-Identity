using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Register;

public class RegisterUserCommand : Command<LoginResponseDTO>
{
    public string Name { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string ConfirmPassword { get; private set; }

    public RegisterUserCommand(string name, string cpf,
                                      string email, string password,
                                      string confirmPassword)
    {
        AggregateId = Guid.NewGuid();
        Name = name;
        Cpf = cpf;
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}
