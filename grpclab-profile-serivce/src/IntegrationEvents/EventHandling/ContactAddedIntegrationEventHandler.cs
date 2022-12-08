using GRPCLab.BuildingBlocks.EventBus.Abstractions;
using GRPCLab.ProfileService.IntegrationEvents.Events;
using Serilog.Context;

namespace GRPCLab.ProfileService.IntegrationEvents.EventHandling
{
    public class ContactAddedIntegrationEventHandler : IIntegrationEventHandler<ContactAddedIntegrationEvent>
    {
        private readonly ILogger<ContactAddedIntegrationEventHandler> _logger;

        public ContactAddedIntegrationEventHandler(
            ILogger<ContactAddedIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ContactAddedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at ({@IntegrationEvent})", @event.Id, @event);
                await Task.FromResult(0);
            }
        }
    }
}
