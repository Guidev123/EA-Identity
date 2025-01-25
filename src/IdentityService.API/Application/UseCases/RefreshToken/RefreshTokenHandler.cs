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
            return new(false, 400, null, ResponseMessages.INVALID_REFRESH_TOKEN.GetDescription());

        var token = await _token.GetRefreshToken(Guid.Parse(request.RefreshToken));
        if (token is null) return new(false, 400, null, ResponseMessages.INVALID_REFRESH_TOKEN.GetDescription());

        return new(true, 200, await _token.JwtGenerator(token.UserIdentification), ResponseMessages.SUCCESS.GetDescription());
    }
}
