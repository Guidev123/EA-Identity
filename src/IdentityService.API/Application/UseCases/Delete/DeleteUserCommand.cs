using EA.IntegrationEvents.Integration.DeletedUser;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Delete;

public class DeleteUserCommand(Guid id) : Command<DeletedUserIntegrationEvent>
{
    public Guid Id { get; private set; } = id;
}
