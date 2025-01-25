using EA.IntegrationEvents.Integration;
using EA.IntegrationEvents.Integration.DeletedUser;
using IdentityService.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;
using SharedLib.MessageBus;

namespace IdentityService.API.Application.UseCases.Delete;

public sealed class DeleteUserHandler(IMessageBus bus, UserManager<IdentityUser> userManager)
                  : CommandHandler, IRequestHandler<DeleteUserCommand, Response<DeletedUserIntegrationEvent>>
{
    private readonly IMessageBus _bus = bus;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    public async Task<Response<DeletedUserIntegrationEvent>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeletedUserIntegrationEvent(request.Id);

        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null) return new(false, 404, null, ResponseMessages.USER_NOT_FOUND.GetDescription());

        var result = await _bus.RequestAsync<DeletedUserIntegrationEvent, ResponseMessage>(deleteEvent);

        if (result.ValidationResult.IsValid)
        {
            await _userManager.DeleteAsync(user);
            return new(false, 404, null, ResponseMessages.SUCCESS.GetDescription());
        }

        return new(false, 400, null, ResponseMessages.CANT_DELETE_USER.GetDescription());
    }
}
