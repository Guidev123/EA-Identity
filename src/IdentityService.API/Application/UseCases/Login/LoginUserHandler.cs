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
            return new(false, 400, null, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));
        var user = UserMappers.MapToIdentity(request);

        var result = await _signinManager.PasswordSignInAsync(request.Email, request.Password, false, true);
        if (result.Succeeded)
            return new(true, 200, await _token.JwtGenerator(user.Email!), ResponseMessages.SUCCESS.GetDescription());

        if (result.IsLockedOut)
        {
            AddError(validationResult, ResponseMessages.LOCKED_ACCOUNT.GetDescription());
            return new(false, 400, null, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        AddError(validationResult, ResponseMessages.WRONG_CREDENTIALS.GetDescription());
        return new(false, 400, null, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));
    }
}
