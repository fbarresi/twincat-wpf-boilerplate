using System;
using System.Threading.Tasks;
using TwinCAT;

namespace WpfApp.Interfaces.Hardware
{
    public interface IPlc : IDisposable
    {
        public IObservable<ConnectionState> ConnectionState { get; }
        public IObservable<T> CreateNotification<T>(string variable);
        public Task<T> Read<T>(string variable);
        public Task Write<T>(string variable, T value);
    }
}