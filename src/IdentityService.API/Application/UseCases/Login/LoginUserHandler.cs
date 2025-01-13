using IdentityService.API.Application.Mappers;
using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.UseCases.Login;

public sealed class LoginUserHandler(SignInManager<IdentityUser> signinManager,
                                     ITokenService token)
                                   : CommandHandler, IRequestHandler<LoginUserCommand, Response<LoginResponseDTO>>
{
    private readonly SignInManager<IdentityUser> _signinManager = signinManager;
    private readonly ITokenService _token = token;
    public async Task<Response<LoginResponseDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateEntity(new LoginUserValidation(), request);

        if (!validationResult.IsValid)
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        var user = UserMappers.MapToIdentity(request);

        var result = await _signinManager.PasswordSignInAsync(request.Email, request.Password, false, true);
        if (result.Succeeded)
            return new(await _token.JwtGenerator(user.Email!), 200, ErrorsMessage.SUCCESS.GetDescription());

        if (result.IsLockedOut)
        {
            AddError(validationResult, ErrorsMessage.LOCKED_ACCOUNT.GetDescription());
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        AddError(validationResult, ErrorsMessage.WRONG_CREDENTIALS.GetDescription());
        return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
    }
}
