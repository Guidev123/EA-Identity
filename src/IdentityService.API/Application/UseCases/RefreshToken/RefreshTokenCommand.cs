using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.RefreshToken;

public class RefreshTokenCommand(string refreshToken) : Command<LoginResponseDTO>
{
    public string RefreshToken { get; private set; } = refreshToken;
}
