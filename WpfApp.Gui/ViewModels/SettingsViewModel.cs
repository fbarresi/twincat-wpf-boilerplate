using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsProvider settingsProvider;
        public ApplicationSetting Setting { get; }

        public ReactiveCommand<Unit, Unit> SaveSettings { get; set; }

        public SettingsViewModel(ApplicationSetting setting, ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
            Setting = setting;
        }
        public override void Init()
        {
            SaveSettings = ReactiveCommand.CreateFromTask(Save).AddDisposableTo(Disposables);
        }

        private Task Save()
        {
            settingsProvider.SaveSettings();
            return Task.FromResult(Unit.Default);
        }
    }

    internal class DesignSettingsViewModel : SettingsViewModel
    {
        public DesignSettingsViewModel() : base(new ApplicationSetting(), new DesignSettingsProvider())
        {
            Init();
        }
    }
}