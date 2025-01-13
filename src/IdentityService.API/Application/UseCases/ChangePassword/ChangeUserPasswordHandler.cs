using IdentityService.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.UseCases.ChangePassword;

public sealed class ChangeUserPasswordHandler(UserManager<IdentityUser> userManager)
                  : CommandHandler, IRequestHandler<ChangeUserPasswordCommand, Response<ChangeUserPasswordCommand>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    public async Task<Response<ChangeUserPasswordCommand>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateEntity(new ChangeUserPasswordValidation(), request);

        if (!validationResult.IsValid)
        {
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            AddError(validationResult, ErrorsMessage.USER_NOT_FOUND.GetDescription());
            return new(null, 404, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        if (!checkPasswordResult)
        {
            AddError(validationResult, ErrorsMessage.WRONG_CREDENTIALS.GetDescription());
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            AddError(validationResult, ErrorsMessage.CANT_CHANGE_PASSWORD.GetDescription());
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        return new(null, 204, ErrorsMessage.SUCCESS.GetDescription());
    }
}
