using IdentityService.API.Application.UseCases.Login;
using IdentityService.API.Application.UseCases.Register;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Application.Mappers;

public static class UserMappers
{
    public static IdentityUser MapToIdentity(RegisterUserCommand command) => new()
    {
        UserName = command.Email,
        Email = command.Email,
        EmailConfirmed = true
    };
    public static IdentityUser MapToIdentity(LoginUserCommand command) => new()
    {
        UserName = command.Email,
        Email = command.Email,
        EmailConfirmed = true
    };
}
