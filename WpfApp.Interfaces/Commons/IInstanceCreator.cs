using Ninject.Parameters;

namespace WpfApp.Interfaces.Commons
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}