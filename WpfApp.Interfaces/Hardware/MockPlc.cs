using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using TwinCAT;
using WpfApp.Interfaces.Extensions;

namespace WpfApp.Interfaces.Hardware
{
    public class MockPlc : IPlc
    {
        public void Dispose()
        {
        }

        public IObservable<ConnectionState> ConnectionState => new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.Connected);
        public IObservable<T> CreateNotification<T>(string variable)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            return Observable.Interval(TimeSpan.FromSeconds(0.5))
                .Select(_ => random.GetData<T>())
                ;
        }

        public Task<T> Read<T>(string variable)
        {
            return Task.FromResult(default(T));
        }

        public Task Write<T>(string variable, T value)
        {
            return Task.FromResult(Unit.Default);
        }

        public IObservable<object> CreateNotification(string variable)
        {
            return CreateNotification<bool>(variable).Select(i => i as object);
        }
    }
}