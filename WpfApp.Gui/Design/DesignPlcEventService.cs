using System;
using System.Reactive.Linq;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.Design
{
    internal class DesignPlcEventService : IPlcEventService
    {
        public IObservable<PlcEvent[]> ActiveEvents => Observable.Never<PlcEvent[]>();
        public IObservable<PlcEvent> LatestEvent => Observable.Never<PlcEvent>();
    }
}