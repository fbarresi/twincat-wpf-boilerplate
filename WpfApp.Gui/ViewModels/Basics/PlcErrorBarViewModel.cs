using System;
using System.Reactive.Linq;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcErrorBarViewModel : ViewModelBase
    {
        private readonly IPlcEventService eventService;
        private bool hasErrors;
        private PlcEvent error;

        public PlcErrorBarViewModel(IPlcEventService eventService)
        {
            this.eventService = eventService;
        }
        protected override void Initialize()
        {
            eventService.LatestEvent
                .ObserveOnDispatcher()
                .Do(b => HasErrors = (b == null))
                .Where(e => e != null)
                .Do(e => Error = e)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
        }

        public PlcEvent Error
        {
            get => error;
            set
            {
                if (Equals(value, error)) return;
                error = value;
                raisePropertyChanged();
            }
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
        public DesignPlcErrorBarViewModel() : base(new DesignPlcEventService())
        {
            Init();
        }
    }
}