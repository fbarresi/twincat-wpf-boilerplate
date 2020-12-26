using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using DynamicData.Annotations;
using ReactiveUI;
using Serilog;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Gui.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IViewModel
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        private bool disposed;
        private string title;

        public string Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                raisePropertyChanged();
            }
        }

        protected ILogger Logger { get; } = Log.Logger;

        public virtual void Dispose()
        {
            Dispose(true);
        }

        public abstract void Init();

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "Disposables")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            Disposables?.Dispose();
            Disposables = null;
            Logger?.Debug("Disposed {type}", this.GetType().Name);

            disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        // ReSharper disable once InconsistentNaming
        protected void raisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.RaisePropertyChanged(propertyName);
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }
    }
}