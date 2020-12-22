using System;

namespace WpfApp.Interfaces.Ui
{
    public interface IPresentationService
    {
        void SwitchActiveViewModel(IViewModel viewModel);
        void SwitchActiveViewModel(Type viewModelType);
        void SwitchActiveViewModel<T>() where T : IViewModel;
        IObservable<IViewModel> ActiveViewModel { get; }
    }
}