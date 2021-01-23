using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TwinCAT;
using WpfApp.Interfaces.Extensions;
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
        private ConnectionState connectionState;
        private object rawValue;

        public PlcVariableViewModel(IPlcProvider provider)
        {
            this.provider = provider;
        }
        public override void Init()
        {
            variableSubscriptions.AddDisposableTo(Disposables);
            Logger.Debug("PlcVariableViewModel view model initialized!");
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

        public void SetupVariableViewModel(string plcName, string variablePath, string label, string description)
        {
            var subscriptions = new CompositeDisposable();
            
            Logger.Debug($"Setting up variable Plc Variable for {variablePath}");
            
            var plc = plcName == null ? provider.GetHardware() : provider.GetHardware(plcName);
            PlcName = plcName;
            VariablePath = variablePath;
            Label = label;
            Description = description;
            
            plc.ConnectionState.ObserveOnDispatcher()
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

            variableSubscriptions.Disposable = subscriptions;
        }
    }
}