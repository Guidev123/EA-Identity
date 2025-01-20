using EA.IntegrationEvents.Integration.DeletedUser;
using SharedLib.Domain.Messages;

namespace IdentityService.API.Application.UseCases.Delete;

public class DeleteUserCommand : Command<DeletedUserIntegrationEvent>
{
    public DeleteUserCommand(Guid id)
    {
        AggregateId = Guid.NewGuid();
        Id = id;
    }

    public Guid Id { get; private set; }
}
