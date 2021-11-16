using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using WpfApp.Gui.Design;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcErrorDetailsViewModel : ViewModelBase
    {
        private readonly IPlcEventService plcEventService;
        private ObservableCollection<PlcEvent> activeErrors = new ObservableCollection<PlcEvent>();

        public PlcErrorDetailsViewModel(IPlcEventService plcEventService)
        {
            this.plcEventService = plcEventService;
        }
        protected override void Initialize()
        {
            plcEventService.ActiveEvents
                .Do(_ => ActiveErrors.Clear())
                .Do(events => ActiveErrors.AddRange(events))
                .Subscribe()
                .AddDisposableTo(Disposables)
                ;
        }

        public ObservableCollection<PlcEvent> ActiveErrors
        {
            get => activeErrors;
            set
            {
                if (Equals(value, activeErrors)) return;
                activeErrors = value;
                raisePropertyChanged();
            }
        }
    }

    internal class DesignPlcErrorDetailsViewModel : PlcErrorDetailsViewModel
    {
        public DesignPlcErrorDetailsViewModel() : base(new DesignPlcEventService())
        {
            Init();
        }
    }
}