using System;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Interfaces.Commons
{
    public interface IViewModelFactory
    {
        T Create<T>();

        TVm CreateViewModel<T, TVm>(T model);

        TVm CreateViewModel<TVm>();
        IViewModel CreateViewModel(Type viewModelType);
    }
}