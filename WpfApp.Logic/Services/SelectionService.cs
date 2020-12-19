using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp.Interfaces.Services;

namespace WpfApp.Logic.Services
{
    public class SelectionService<T> : ISelectionService<T>
    {
        public IObservable<T> Current => subject.AsObservable();
        private readonly Subject<T> subject = new Subject<T>();

        public void Select(T step)
        {
            subject.OnNext(step);
        }
    }
}