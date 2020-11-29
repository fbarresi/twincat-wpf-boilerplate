using Ninject.Parameters;

namespace WpfApp.Interfaces
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(ConstructorArgument[] arguments);
    }
}