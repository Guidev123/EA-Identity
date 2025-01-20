using IdentityService.API.Application.UseCases.ForgotPassword;
using IdentityService.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.UseCases.ResetPassword;

public sealed class ResetPasswordHandler(UserManager<IdentityUser> userManager)
                                       : CommandHandler,
                                         IRequestHandler<ResetPasswordCommand,
                                         Response<ResetPasswordCommand>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    public async Task<Response<ResetPasswordCommand>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateEntity(new ResetPasswordValidation(), request);
        if (!validationResult.IsValid)
            return new(null, 400, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            AddError(validationResult, ResponseMessages.USER_NOT_FOUND.GetDescription());
            return new(null, 404, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToArray();
            return new(null, 400, ResponseMessages.ERROR.GetDescription(), errors);
        }

        return new(null, 204, ResponseMessages.SUCCESS.GetDescription());
    }
}
