using Autofac;
using GRPCLab.ContactService.IntegrationEvents.EventHandling;

namespace GRPCLab.ContactService.Infrastructures.Modules
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContactAddedIntegrationEventHandler>();
        }
    }
}
