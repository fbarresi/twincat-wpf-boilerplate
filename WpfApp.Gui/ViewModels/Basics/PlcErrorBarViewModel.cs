using System;
using System.Reactive.Linq;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcErrorBarViewModel : ViewModelBase
    {
        private readonly IPlcErrorService plcErrorService;
        private bool hasErrors;

        public PlcErrorBarViewModel(IPlcErrorService plcErrorService)
        {
            this.plcErrorService = plcErrorService;
        }
        protected override void Initialize()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(l => l % 2 == 0)
                .ObserveOnDispatcher()
                .Do(b => HasErrors = b)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
        }

        public bool HasErrors
        {
            get => hasErrors;
            set
            {
                if (value == hasErrors) return;
                hasErrors = value;
                raisePropertyChanged();
            }
        }
    }

    internal class DesignPlcErrorBarViewModel : PlcErrorBarViewModel
    {
        public DesignPlcErrorBarViewModel() : base(new DesignPlcErrorService())
        {
            Init();
        }
    }
}