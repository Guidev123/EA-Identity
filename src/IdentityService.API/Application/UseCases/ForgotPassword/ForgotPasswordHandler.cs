using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Application.UseCases.ForgotPassword;

public class ForgotPasswordHandler(UserManager<IdentityUser> userManager,
                                   IEmailService emailService)
                                 : CommandHandler,
                                   IRequestHandler<ForgotPasswordCommand,
                                   Response<ForgotPasswordCommand>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    public async Task<Response<ForgotPasswordCommand>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateEntity(new ForgotPasswordValidation(), request);
        if (!validationResult.IsValid)
            return new(null, 400, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            AddError(validationResult, ResponseMessages.USER_NOT_FOUND.GetDescription());
            return new(null, 404, ResponseMessages.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var param = new Dictionary<string, string?>
        {
            {"token", token },
            {"email", request.Email}
        };

        var callback = QueryHelpers.AddQueryString(request.ClientUriToResetPassword, param);

        var message = new EmailMessageDTO(user.Email!, "Reset password token", callback);

        await _emailService.SendAsync(message);

        return new(null, 204, ResponseMessages.SUCCESS.GetDescription());
    }
}
