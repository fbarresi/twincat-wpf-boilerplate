using System;
using System.Reactive.Linq;
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
        private string _test = "woow!";

        public string Test
        {
            get => _test;
            set{
                if (value == _test) return;
                _test = value;
                raisePropertyChanged();
            }
        }

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
                .Do(i => Test += i)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            Logger.Debug("Main view model initialized!");
        }
    }
}