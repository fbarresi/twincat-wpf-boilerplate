using System;
using WpfApp.Interfaces.Models;

namespace WpfApp.Interfaces.Services
{
    public interface IPlcEventService
    {
        public IObservable<PlcEvent[]> ActiveEvents { get; }
        public IObservable<PlcEvent> LatestEvent { get; }
    }
}