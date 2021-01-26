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

        public MainViewModel()
        {
            Title = "Main View";
        }
        protected override void Initialize()
        {
            Logger.Debug("Main view initialized!");
        }

    }

    internal class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel() : base()
        {
            Init();
        }
    }
}