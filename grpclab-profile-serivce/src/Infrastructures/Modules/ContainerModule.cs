using Autofac;
using GRPCLab.ProfileService.IntegrationEvents.EventHandling;

namespace GRPCLab.ProfileService.Infrastructures.Modules
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContactAddedIntegrationEventHandler>();
        }
    }
}
