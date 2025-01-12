using IdentityService.API.DTOs;
using MediatR;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Response<LoginResponseDTO>>
{
    public Task<Response<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
