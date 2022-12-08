using GRPCLab.BuildingBlocks.EventBus.Events;

namespace GRPCLab.ProfileService.IntegrationEvents.Events
{
    public record ContactAddedIntegrationEvent : IntegrationEvent
    {
        public int ContactId { get; init; }

        public ContactAddedIntegrationEvent(int contactId)
            => ContactId = contactId;
    }

}
