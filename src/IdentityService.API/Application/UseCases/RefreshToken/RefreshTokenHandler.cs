using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using MediatR;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.UseCases.RefreshToken;

public sealed class RefreshTokenHandler(ITokenService token)
                  : CommandHandler, IRequestHandler<RefreshTokenCommand, Response<LoginResponseDTO>>
{
    private readonly ITokenService _token = token;
    public async Task<Response<LoginResponseDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return new(null, 400, ErrorsMessage.INVALID_REFRESH_TOKEN.GetDescription());

        var token = await _token.GetRefreshToken(Guid.Parse(request.RefreshToken));
        if (token is null) return new(null, 400, ErrorsMessage.INVALID_REFRESH_TOKEN.GetDescription());

        return new(await _token.JwtGenerator(token.UserIdentification), 200, ErrorsMessage.SUCCESS.GetDescription());
    }
}
