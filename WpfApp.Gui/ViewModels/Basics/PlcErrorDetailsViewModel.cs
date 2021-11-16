using WpfApp.Gui.Design;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcErrorDetailsViewModel : ViewModelBase
    {

        public PlcErrorDetailsViewModel()
        {
        }
        protected override void Initialize()
        {
            
        }
    }

    internal class DesignPlcErrorDetailsViewModel : PlcErrorDetailsViewModel
    {
        public DesignPlcErrorDetailsViewModel() : base()
        {
            Init();
        }
    }
}