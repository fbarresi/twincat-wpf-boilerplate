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
        private bool sideMenuOpen;
        private ObservableAsPropertyHelper<ConnectionState> helper;
        private ViewModelBase activeViewModel;
        private CultureInfo selectedLanguage;

        public ReactiveCommand<Unit, Unit> ToggleMenu { get; set; }
        public ReactiveCommand<Type, Unit> SwitchToViewModel { get; set; }
        public ReactiveCommand<string, Unit> ChangeLanguageCmd { get; set; }

        public CultureInfo SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (value == selectedLanguage) return;
                selectedLanguage = value;
                raisePropertyChanged();
            }
        }

        public CultureInfo[] SupportedCultures => settingRoot.CultureSettings.SupportedCultures;
        
        public bool SideMenuOpen
        {
            get => sideMenuOpen;
            set
            {
                if(value == sideMenuOpen) return;
                sideMenuOpen = value;
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
            SelectedLanguage = settingRoot.CultureSettings.SelectedCulture;
            this.WhenAnyValue(vm => vm.SelectedLanguage)
                .ObserveOnDispatcher()
                .Do(ChangeCurrentCulture)
                .Subscribe()
                .AddDisposableTo(Disposables);

            ChangeLanguageCmd = ReactiveCommand.CreateFromTask<string>(ChangeCurrentCulture)
                .AddDisposableTo(Disposables);

            var plc = provider.GetHardware();

            helper = plc.ConnectionState.ToProperty(this, vm => vm.ConnectionState, ConnectionState.None);

            Logger.Debug("Main view model initialized!");

            SetInitialViewModel();

            SetupContentPresenter();
            
            SwitchToViewModel = ReactiveCommand.CreateFromTask<Type, Unit>(SwitchTo)
                .AddDisposableTo(Disposables);

            ToggleMenu = ReactiveCommand.CreateFromTask(ToggleSideMenuOpen)
                .AddDisposableTo(Disposables);
            
        }

        private Task<Unit> ToggleSideMenuOpen()
        {
            SideMenuOpen = !SideMenuOpen;
            return Task.FromResult(Unit.Default);
        }

        private Task ChangeCurrentCulture(string culture)
        {
            settingRoot.CultureSettings.SelectedCulture = CultureInfo.GetCultureInfo(culture);
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(culture);
            return Task.FromResult(Unit.Default);
        }
        
        private void ChangeCurrentCulture(CultureInfo culture)
        {
            Logger.Debug("Setting current culture to {culture}", culture);
            settingRoot.CultureSettings.SelectedCulture = culture;
            LocalizeDictionary.Instance.Culture = culture;
        }

        private void InitializeLocalization()
        {
            LocalizeDictionary.Instance.Culture = settingRoot.CultureSettings.SelectedCulture;
        }

        private Task<Unit> SwitchTo(Type type)
        {
            presentationService.SwitchActiveViewModel(type);
            SideMenuOpen = false;
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
            presentationService.SwitchActiveViewModel<MainViewModel>();
        }

        public ViewModelBase ActiveViewModel
        {
            get => activeViewModel;
            set 
            {
                if (value == activeViewModel) return;
                activeViewModel = value;
                raisePropertyChanged();
            }
        }

    }
}