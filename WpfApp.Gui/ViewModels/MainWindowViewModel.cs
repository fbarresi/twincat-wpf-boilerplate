using System;
using System.Reactive.Linq;
using ReactiveUI;
using TwinCAT;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IViewModelFactory viewModelFactory;
        private readonly IPlcProvider provider;
        private readonly ApplicationSetting setting;
        private string _connectionState = "woow!";
        private string _test;
        private ObservableAsPropertyHelper<ConnectionState> helper;
        private PageViewModel _pageViewModel;

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

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IPlcProvider provider, ApplicationSetting setting)
        {
            this.viewModelFactory = viewModelFactory;
            this.provider = provider;
            this.setting = setting;
        }
        public override void Init()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Do(i => Test = "Test "+i)
                .Subscribe()
                .AddDisposableTo(Disposables);

            var plc = provider.GetHardware(setting.PlcName);

            helper = plc.ConnectionState.ToProperty(this, vm => vm.ConnectionState, ConnectionState.None);

            Logger.Debug("Main view model initialized!");

            PageViewModel = viewModelFactory.CreateViewModel<PageViewModel>();
            PageViewModel.AddDisposableTo(Disposables);
            
        }

        public PageViewModel PageViewModel
        {
            get => _pageViewModel;
            set
            {
                if (value == _pageViewModel) return;
                _pageViewModel = value;
                raisePropertyChanged();
            }
        }
    }
}