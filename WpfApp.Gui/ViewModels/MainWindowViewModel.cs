using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using TwinCAT;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;
using WpfApp.Interfaces.Ui;
using WPFLocalizeExtension.Engine;

namespace WpfApp.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IPlcProvider provider;
        private readonly IPresentationService presentationService;
        private readonly SettingRoot settingRoot;
        private string _test;
        private ObservableAsPropertyHelper<ConnectionState> helper;
        private ViewModelBase _activeViewModel;

        public ReactiveCommand<Type, Unit> SwitchToViewModel { get; set; }
        public ReactiveCommand<string, Unit> ChangeLanguageCmd { get; set; }

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

        public MainWindowViewModel(IPlcProvider provider, IPresentationService presentationService, SettingRoot settingRoot)
        {
            this.provider = provider;
            this.presentationService = presentationService;
            this.settingRoot = settingRoot;
        }
        public override void Init()
        {
            InitializeLocalization();
            
            ChangeLanguageCmd = ReactiveCommand.CreateFromTask<string>(ChangeCurrentCulture)
                .AddDisposableTo(Disposables);

            var plc = provider.GetHardware();

            helper = plc.ConnectionState.ToProperty(this, vm => vm.ConnectionState, ConnectionState.None);

            Logger.Debug("Main view model initialized!");

            SetInitialViewModel();

            SetupContentPresenter();
            
            SwitchToViewModel = ReactiveCommand.CreateFromTask<Type, Unit>(SwitchTo)
                .AddDisposableTo(Disposables);
        }

        private Task ChangeCurrentCulture(string culture)
        {
            settingRoot.CultureSettings.SelectedCulture = CultureInfo.GetCultureInfo(culture);
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(culture);
            return Task.FromResult(Unit.Default);
        }

        private void InitializeLocalization()
        {
            LocalizeDictionary.Instance.Culture = settingRoot.CultureSettings.SelectedCulture;
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
            presentationService.SwitchActiveViewModel<SignalViewModel>();
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