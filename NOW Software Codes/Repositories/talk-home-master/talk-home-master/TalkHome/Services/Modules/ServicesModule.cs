using Autofac;

namespace TalkHome.Services.Modules
{
    /// <summary>
    /// Autofac module for registering the Services
    /// </summary>
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JWTService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AccountService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<ContentService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<PaymentService>().AsImplementedInterfaces().InstancePerLifetimeScope();
      
            builder.RegisterType<Pay360Service>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<PayPalService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            
            builder.RegisterType<ActiveCampaignService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AirTimeTransferService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<BusinessIntelligenceService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<PortService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
