using EA.IntegrationEvents.Integration;
using EA.IntegrationEvents.Integration.RegisteredUser;
using IdentityService.API.Application.Mappers;
using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;
using SharedLib.MessageBus;

namespace IdentityService.API.Application.UseCases.Register;

public sealed class RegisterUserHandler(UserManager<IdentityUser> userManager,
                                        ITokenService token,
                                        IMessageBus bus)
                                      : CommandHandler,
                                        IRequestHandler<RegisterUserCommand, Response<LoginResponseDTO>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly ITokenService _token = token;
    private readonly IMessageBus _bus = bus;

    public async Task<Response<LoginResponseDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateEntity(new RegisterUserValidation(), request);

        if (!validationResult.IsValid)
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));

        var user = UserMappers.MapToIdentity(request);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var customerResult = await RegisterCustomer(request);

            if (!customerResult.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(customerResult.ValidationResult));
            }

            return new(await _token.JwtGenerator(user.Email!), 201, ErrorsMessage.SUCCESS.GetDescription());
        }

        return new(null, 400, ErrorsMessage.ERROR.GetDescription(), result.Errors.Select(e => e.Description).ToArray());
    }

    private async Task<ResponseMessage> RegisterCustomer(RegisterUserCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        var registeredUser = new RegisteredUserIntegrationEvent(Guid.Parse(user!.Id), command.Name, command.Email, command.Cpf);

        try
        {
            return await _bus.RequestAsync<RegisteredUserIntegrationEvent, ResponseMessage>(registeredUser);
        }

        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }
}
