using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Ninject;
using ReactiveUI;
using Serilog;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Gui.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IViewModel
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        private bool disposed;
        private string title;
        private ObservableAsPropertyHelper<User> currentUserHelper;
        private ObservableAsPropertyHelper<bool> loggedInHelper;

        [Inject]
        public IUserService UserService { get; set; }
        
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

        public void Init()
        {
            Initialize();
            InitializeBaseElements();
        }
        
        protected abstract void Initialize();

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

        private void InitializeBaseElements()
        {
            currentUserHelper = UserService?.CurrentUser.ToProperty(this, vm => vm.CurrentUser);
            currentUserHelper.AddDisposableTo(Disposables);

            loggedInHelper = UserService?.CurrentUser.Select(u => u != null).ToProperty(this, vm => vm.LoggedIn);
            loggedInHelper.AddDisposableTo(Disposables);
        }

        public bool LoggedIn => loggedInHelper.Value;

        public User CurrentUser => currentUserHelper.Value;
    }
}