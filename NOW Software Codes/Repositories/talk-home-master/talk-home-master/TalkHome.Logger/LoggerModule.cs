using Autofac;

namespace TalkHome.Logger
{
    /// <summary>
    /// Autofac module for registering the Services
    /// </summary>
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggerService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
