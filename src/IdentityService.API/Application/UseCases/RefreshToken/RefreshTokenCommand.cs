using IdentityService.API.DTOs;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.RefreshToken;

public class RefreshTokenCommand : Command<LoginResponseDTO>
{
    public string RefreshToken { get; private set; }

    public RefreshTokenCommand(string refreshToken)
    {
        AggregateId = Guid.NewGuid();
        RefreshToken = refreshToken;
    }
}
