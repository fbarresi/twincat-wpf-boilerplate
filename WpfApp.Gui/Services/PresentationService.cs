using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Gui.Services
{
    public class PresentationService : IPresentationService, IDisposable
    {
        private readonly IViewModelFactory viewModelFactory;

        private readonly BehaviorSubject<IViewModel> activeViewModelSubject = new BehaviorSubject<IViewModel>(null);

        public PresentationService(IViewModelFactory viewModelFactory)
        {
            this.viewModelFactory = viewModelFactory;
        }
        
        public void SwitchActiveViewModel(IViewModel viewModel)
        {
            activeViewModelSubject.Value?.Dispose();
            activeViewModelSubject.OnNext(viewModel);
        }

        public void SwitchActiveViewModel(Type viewModelType)
        {
            var viewModel = viewModelFactory.CreateViewModel(viewModelType);
            SwitchActiveViewModel(viewModel);
        }

        public void SwitchActiveViewModel<T>() where T : IViewModel
        {
            var viewModel = viewModelFactory.CreateViewModel<T>();
            SwitchActiveViewModel(viewModel);
        }

        public IObservable<IViewModel> ActiveViewModel => activeViewModelSubject.AsObservable();

        public void Dispose()
        {
            activeViewModelSubject.Value?.Dispose();
            activeViewModelSubject.OnCompleted();
            activeViewModelSubject.Dispose();
        }
    }
}