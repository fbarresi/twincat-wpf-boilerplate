using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Serilog;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Hardware;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Logic.Hardware
{
    public class BeckhoffPlc : IPlc
    {
        private readonly PlcSetting settings;
        private readonly BehaviorSubject<ConnectionState> connectionStateSubject = new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.None);
        private readonly BehaviorSubject<AdsState> adsStateSubject = new BehaviorSubject<AdsState>(TwinCAT.Ads.AdsState.Init);
        public IObservable<AdsState> AdsState => adsStateSubject.AsObservable();
        public readonly CompositeDisposable Disposables = new CompositeDisposable();
        
        [Inject]
        public ILogger Logger { get; set; }
        
        public BeckhoffPlc(PlcSetting settings)
        {
            this.settings = settings;
            Client = new AdsClient();
        }

        public AdsClient Client { get; }

        private void InitializeBeckhoff()
        {
            if (string.IsNullOrEmpty(settings.AmsNetId))
            {
                Client.Connect(settings.Port); //Connect ads locally
            }
            else
            {
                Client.Connect(settings.AmsNetId, settings.Port);
            }
        }

        public bool Initialize()
        {
            try
            {
                Logger?.Information($"Connecting with Beckhoff at '{settings.AmsNetId}:{settings.Port}'...");

                InitializeBeckhoff();
            }
            catch (Exception e)
            {
                Logger?.Error(e, "Error while initializing beckhoff");
            }
            
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Do(_ => CheckConnectionHealth())
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            connectionStateSubject
                .DistinctUntilChanged()
                .Do(state => Logger?.Debug($"Connection state changed to '{state}'"))
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            adsStateSubject
                .DistinctUntilChanged()
                .Do(state => Logger?.Debug($"Ads state changed to '{state}'"))
                .Do(UpdateSymbols)
                .Subscribe()
                .AddDisposableTo(Disposables);
            
            connectionStateSubject.AddDisposableTo(Disposables);
            adsStateSubject.AddDisposableTo(Disposables);
            symbolsSubject.AddDisposableTo(Disposables);
            
            return true;
        }

        private void CheckConnectionHealth()
        {
            try
            {
                if (!Client.IsConnected)
                {
                    InitializeBeckhoff();
                }
                var state = Client.ReadState();
                connectionStateSubject.OnNext(TwinCAT.ConnectionState.Connected);
                adsStateSubject.OnNext(state.AdsState);
            }
            catch (Exception)
            {
                connectionStateSubject.OnNext(TwinCAT.ConnectionState.Lost);
                adsStateSubject.OnNext(TwinCAT.Ads.AdsState.Invalid);
                Client.Disconnect();
            }
        }

        private void UpdateSymbols(AdsState state)
        {
            if (state == TwinCAT.Ads.AdsState.Run)
            {
                Logger?.Debug($"Update symbols on beckhoff change to {state}");

                var loader = SymbolLoaderFactory.Create(Client, new SymbolLoaderSettings(SymbolsLoadMode.Flat));
                symbolsSubject.OnNext(loader.Symbols);
            }
            else
            {
                Logger?.Debug($"Deleting symbols on beckhoff state change to {state}");
                symbolsSubject.OnNext(null);
            }
        }
        private readonly BehaviorSubject<ISymbolCollection<ISymbol>> symbolsSubject = new BehaviorSubject<ISymbolCollection<ISymbol>>(null);
        public IObservable<ISymbolCollection<ISymbol>> Symbols => symbolsSubject.AsObservable();
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public IObservable<ConnectionState> ConnectionState => connectionStateSubject.AsObservable();
        
        public IObservable<T> CreateNotification<T>(string variable)
        {
            return connectionStateSubject
                    .Where(connectionStates => connectionStates == TwinCAT.ConnectionState.Connected)
                    .Select(_ => ObserveVariable<T>(variable))
                    .Switch()
                    .Retry()
                ;
        }

        private IObservable<T> ObserveVariable<T>(string variable)
        {
            var symbol = TryGetSymbol(variable);

            Logger?.Debug(
                $"Creating beckhoff notification for '{variable}' of type {typeof(T)} (internally {symbol.DataType.Name})");

            IObservable<object> observable = ((IValueSymbol) symbol).WhenValueChanged();

            return observable 
                    .Select(obj =>
                    {
                        if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(TimeSpan))
                            return obj.TryConvertToDotNetManagedType();
                        return obj;
                    })
                    .Select(obj => obj.ConvertTo<T>())
                ;
        }

        private ISymbol TryGetSymbol(string variableName)
        {
            ISymbol symbol;
            if (symbolsSubject.Value.TryGetInstance(variableName, out symbol)) return symbol;
            throw new InvalidOperationException($"No symbol found for variable name {variableName}");
        }
        
        public Task<T> Read<T>(string variable)
        {
            return Task.Run(() =>
                {
                    var symbolInfo = Client.ReadSymbol(variable);
                    if (symbolInfo.DataType.IsPrimitive)
                    {
                        var obj = Client.ReadValue(variable, typeof(T));
                        return (T) Convert.ChangeType(obj, typeof(T));
                    }
                    var handle = Client.CreateVariableHandle(variable);
                    var value = (T)Client.ReadAny(handle, typeof(T));
                    Client.DeleteVariableHandle(handle);
                    return value;
                }
            );
        }

        public Task Write<T>(string variable, T value)
        {
            return Task.Run(() =>
            {
                var symbolInfo = Client.ReadSymbol(variable);
                if(symbolInfo.DataType.IsPrimitive)
                    Client.WriteValue(symbolInfo, value);
                else
                {
                    Logger?.Error("Unable to write '{variable}' because not primitive", variable);
                }
            }, CancellationToken.None);
        }
    
    }
}