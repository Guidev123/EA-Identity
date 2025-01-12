using IdentityService.API.DTOs;
using MediatR;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<Response<LoginResponseDTO>>;

