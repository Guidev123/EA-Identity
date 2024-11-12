using EA.CommonLib.MessageBus.Integration;

namespace IdentityService.API.IntegrationEvents
{
    public class RegisteredUserIntegrationEvent : IntegrationEvent
    {
        public RegisteredUserIntegrationEvent(Guid id, string name, string email, string cpf)
        {
            AggregateId = id;
            Id = id;
            Name = name;
            Email = email;
            Cpf = cpf;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Cpf{ get; private set; }
    }
}
