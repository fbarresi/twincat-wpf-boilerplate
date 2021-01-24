using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI;
using TwinCAT;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Hardware;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.ViewModels.Basics
{
    public class PlcVariableViewModel : ViewModelBase
    {
        private readonly IPlcProvider provider;
        private string variablePath = string.Empty;
        private string plcName;
        private string label;
        private string description;
        private readonly SerialDisposable variableSubscriptions = new SerialDisposable();
        private readonly BehaviorSubject<bool> canSetVariable = new BehaviorSubject<bool>(false);
        private ConnectionState connectionState;
        private object rawValue;
        private IPlc plc;
        private object setParameter;
        private string valueFormat;

        public ReactiveCommand<Unit, Unit> SetVariable { get; set; }
        public PlcVariableViewModel(IPlcProvider provider)
        {
            this.provider = provider;
        }
        public override void Init()
        {
            variableSubscriptions.AddDisposableTo(Disposables);

            SetVariable = ReactiveCommand
                    .CreateFromTask<Unit, Unit>(WriteVariable)
                    .AddDisposableTo(Disposables)
                    ;
            
            Logger.Debug("PlcVariableViewModel view model initialized!");
        }

        private async Task<Unit> WriteVariable(Unit arg)
        {
            if (plc == null) return Unit.Default;
            await plc.Write(VariablePath, SetParameter);
            return Unit.Default;
        }

        public string VariablePath
        {
            get => variablePath;
            set 
            { 
                if (value == variablePath) return;
                variablePath = value;
                raisePropertyChanged();
            }
        }

        public string PlcName
        {
            get => plcName;
            set 
            { 
                if (value == plcName) return;
                plcName = value;
                raisePropertyChanged();
            }
        }

        public string Label
        {
            get => label;
            set 
            { 
                if (value == label) return;
                label = value;
                raisePropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            set 
            { 
                if (value == description) return;
                description = value;
                raisePropertyChanged();
            }
        }

        public object RawValue
        {
            get => rawValue;
            set 
            { 
                if (value == rawValue) return;
                rawValue = value;
                raisePropertyChanged();
            }
        }

        public ConnectionState ConnectionState
        {
            get => connectionState;
            set 
            { 
                if (value == connectionState) return;
                connectionState = value;
                raisePropertyChanged();
            }
        }

        public object SetParameter
        {
            get => setParameter;
            set
            {
                if (Equals(value, setParameter)) return;
                setParameter = value;
                raisePropertyChanged();
            }
        }

        public string ValueFormat
        {
            get => valueFormat;
            set
            {
                if (value == valueFormat) return;
                valueFormat = value;
                raisePropertyChanged();
            }
        }

        public void SetupVariableViewModel(string plcName, string variablePath, string label, string description,
            object setParameter, string valueFormat)
        {
            var subscriptions = new CompositeDisposable();
            
            Logger.Debug($"Setting up variable Plc Variable for {variablePath}");
            
            plc = plcName == null ? provider.GetHardware() : provider.GetHardware(plcName);
            PlcName = plcName;
            VariablePath = variablePath;
            Label = label;
            Description = description;
            SetParameter = setParameter;
            ValueFormat = valueFormat;
            
            plc.ConnectionState
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .Do(cs => ConnectionState = cs)
                .Subscribe()
                .AddDisposableTo(subscriptions)
                ;
            
            plc.CreateNotification(variablePath)
                .ObserveOnDispatcher()
                .Do(r => RawValue = r)
                .Subscribe()
                .AddDisposableTo(subscriptions)
                ;

            plc.ConnectionState
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .Select(state => state == ConnectionState.Connected)
                .Subscribe(canSetVariable.OnNext)
                .AddDisposableTo(Disposables)
                ;
            
            variableSubscriptions.Disposable = subscriptions;
        }
    }
}