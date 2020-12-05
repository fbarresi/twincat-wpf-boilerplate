using System.Reactive.Linq;
using ReactiveUI;
using TwinCAT;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        private readonly IPlcProvider provider;
        private readonly ApplicationSetting setting;
        private ObservableAsPropertyHelper<bool> helper;

        public PageViewModel(IPlcProvider provider, ApplicationSetting setting)
        {
            this.provider = provider;
            this.setting = setting;
        }
        public override void Init()
        {
            var plc = provider.GetHardware(setting.PlcName);

            helper = plc.CreateNotification<bool>(setting.ToggleSignalName)
                        .ToProperty(this, vm => vm.Toggle, true);

            Logger.Debug("Page view model initialized!");
        }

        public bool Toggle => helper.Value;
    }
}