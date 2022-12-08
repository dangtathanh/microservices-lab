using GRPCLab.BuildingBlocks.EventBus.Events;

namespace GRPCLab.ContactService.IntegrationEvents.Events
{
    public record ContactAddedIntegrationEvent : IntegrationEvent
    {
        public int ContactId { get; init; }

        public ContactAddedIntegrationEvent(int contactId)
            => ContactId = contactId;
    }

}
