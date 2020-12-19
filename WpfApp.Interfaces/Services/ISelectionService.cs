using System;

namespace WpfApp.Interfaces.Services
{
    public interface ISelectionService<T>
    {
        IObservable<T> Current { get; }
        void Select(T step);
    }
}