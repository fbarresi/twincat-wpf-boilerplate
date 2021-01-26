using WpfApp.Gui.Design;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcErrorDetailsViewModel : ViewModelBase
    {
        private readonly IPlcErrorService plcErrorService;

        public PlcErrorDetailsViewModel(IPlcErrorService plcErrorService)
        {
            this.plcErrorService = plcErrorService;
        }
        protected override void Initialize()
        {
            
        }
    }

    internal class DesignPlcErrorDetailsViewModel : PlcErrorDetailsViewModel
    {
        public DesignPlcErrorDetailsViewModel() : base(new DesignPlcErrorService())
        {
            Init();
        }
    }
}