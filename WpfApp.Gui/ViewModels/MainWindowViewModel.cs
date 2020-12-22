using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using TwinCAT;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IViewModelFactory viewModelFactory;
        private readonly IPlcProvider provider;
        private readonly IPresentationService presentationService;
        private string _test;
        private ObservableAsPropertyHelper<ConnectionState> helper;
        private ViewModelBase _activeViewModel;

        public ReactiveCommand<Type, Unit> SwitchToViewModel { get; set; }
        
        public string Test
        {
            get => _test;
            set
            {
                if(value == _test) return;
                _test = value;
                raisePropertyChanged();    
            }
        }

        public ConnectionState ConnectionState => helper?.Value ?? ConnectionState.None;

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IPlcProvider provider, IPresentationService presentationService)
        {
            this.viewModelFactory = viewModelFactory;
            this.provider = provider;
            this.presentationService = presentationService;
        }
        public override void Init()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Do(i => Test = "Test "+i)
                .Subscribe()
                .AddDisposableTo(Disposables);

            var plc = provider.GetHardware();

            helper = plc.ConnectionState.ToProperty(this, vm => vm.ConnectionState, ConnectionState.None);

            Logger.Debug("Main view model initialized!");

            SetInitialViewModel();

            SetupContentPresenter();
            
            SwitchToViewModel = ReactiveCommand.CreateFromTask<Type, Unit>(SwitchTo)
                .AddDisposableTo(Disposables);
        }

        private Task<Unit> SwitchTo(Type type)
        {
            presentationService.SwitchActiveViewModel(type);
            return Task.FromResult(Unit.Default);
        }

        private void SetupContentPresenter()
        {
            presentationService.ActiveViewModel
                .Where(vm => vm != null)
                .ObserveOnDispatcher()
                .Do(vm => ActiveViewModel = vm as ViewModelBase)
                .Subscribe()
                .AddDisposableTo(Disposables);
        }

        private void SetInitialViewModel()
        {
            presentationService.SwitchActiveViewModel<PageViewModel>();
        }

        public ViewModelBase ActiveViewModel
        {
            get => _activeViewModel;
            set 
            {
                if (value == _activeViewModel) return;
                _activeViewModel = value;
                raisePropertyChanged();
            }
        }
    }
}