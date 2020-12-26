using System.Reactive.Linq;
using ReactiveUI;
using TwinCAT;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class SignalViewModel : ViewModelBase
    {
        private readonly IPlcProvider provider;
        private readonly ApplicationSetting setting;
        private ObservableAsPropertyHelper<bool> helper;

        public SignalViewModel(IPlcProvider provider, ApplicationSetting setting)
        {
            this.provider = provider;
            this.setting = setting;
            Title = "Test Page";
        }
        public override void Init()
        {
            var plc = provider.GetHardware();

            helper = plc.CreateNotification<bool>(setting.ToggleSignalName)
                        .ToProperty(this, vm => vm.Toggle, true);

            Logger.Debug("Page view model initialized!");
        }

        public bool Toggle => helper.Value;
    }

    internal class DesignSignalViewModel : SignalViewModel
    {
        public DesignSignalViewModel() : base(new DesignPlcProvider(), new ApplicationSetting())
        {
            Init();
        }
    }
}