namespace WpfApp.Gui.ViewModels
{
    public class GraphViewModel: ViewModelBase
    {
        public override void Init()
        {
            Logger?.Debug("Initialized Graph view");
        }
    }

    internal class DesignGraphViewModel : GraphViewModel
    {
        public DesignGraphViewModel() : base()
        {
            Init();
        }
    }
}