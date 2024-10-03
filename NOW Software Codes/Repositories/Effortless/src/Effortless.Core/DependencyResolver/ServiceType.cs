namespace Effortless.Core.DependencyResolver;

public abstract class ServiceType
{
    public interface IScoped { }
    public interface ISingleton { }
    public interface ITransient { }
}
