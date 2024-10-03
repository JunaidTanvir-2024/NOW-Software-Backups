using Autofac;

namespace TalkHome.WebServices.Modules
{
    /// <summary>
    /// Autofac module for registering the WebServices
    /// </summary>
    public class WebServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<TalkHomeWebService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<AddressIoWebService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<IpInfoWebService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<TalkHomeAppWebService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<PaymentWebService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
