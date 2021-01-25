using System.Reactive.Linq;
using ReactiveUI;
using TwinCAT;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IPlcProvider provider;
        private readonly ApplicationSetting setting;
        private ObservableAsPropertyHelper<bool> helper;

        public MainViewModel(IPlcProvider provider, ApplicationSetting setting)
        {
            this.provider = provider;
            this.setting = setting;
            Title = "Main View";
        }
        protected override void Initialize()
        {
            var plc = provider.GetHardware();

            helper = plc.CreateNotification<bool>(setting.ToggleSignalName)
                        .ToProperty(this, vm => vm.Toggle, true);

            Logger.Debug("SignalViewModel initialized!");
        }

        public bool Toggle => helper.Value;
    }

    internal class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel() : base(new DesignPlcProvider(), new ApplicationSetting())
        {
            Init();
        }
    }
}