using System;
using Ninject;
using Ninject.Parameters;
using WpfApp.Gui.ViewModels;
using WpfApp.Gui.ViewModels.Basics;
using WpfApp.Interfaces;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Ui;
using IInitializable = WpfApp.Interfaces.Commons.IInitializable;

namespace WpfApp.Gui
{
    public class ViewModelLocator : IViewModelFactory, IInstanceCreator
    {
        protected readonly IKernel Kernel;

        private static ViewModelLocator s_Instance;

        public ViewModelLocator()
        {
            BindServices();
        }

        private void BindServices()
        {
            Kernel.Bind<MainWindowViewModel>().To<MainWindowViewModel>();
        }

        public ViewModelLocator(IKernel kernel)
        {
            Kernel = kernel;
            kernel.Bind<IViewModelFactory, IInstanceCreator>().ToConstant(this);
            BindServices();
        }

        public static IInstanceCreator DesignInstanceCreator => s_Instance ?? (s_Instance = new ViewModelLocator());

        public static IViewModelFactory DesignViewModelFactory => s_Instance ?? (s_Instance = new ViewModelLocator());

        public MainWindowViewModel MainWindowViewModel => Kernel.Get<MainWindowViewModel>();

        public PlcVariableViewModel PlcVariableViewModel => CreateViewModel<PlcVariableViewModel>();

        public T Create<T>()
        {
            var newObject = Kernel.Get<T>();
            InitializeInitialziable(newObject as IInitializable);
            return newObject;
        }

        public T CreateInstance<T>(ConstructorArgument[] arguments)
        {
            var vm = Kernel.Get<T>(arguments);
            InitializeInitialziable(vm as IInitializable);
            return vm;
        }

        public TVm CreateViewModel<T, TVm>(T model)
        {
            var vm = Kernel.Get<TVm>(new ConstructorArgument(@"model", model));
            InitializeInitialziable(vm as IInitializable);
            return vm;
        }

        public TVm CreateViewModel<TVm>()
        {
            var vm = Kernel.Get<TVm>();
            InitializeInitialziable(vm as IInitializable);
            return vm;
        }
        
        public IViewModel CreateViewModel(Type viewModelType)
        {
            var vm = Kernel.Get(viewModelType) as IViewModel;
            InitializeInitialziable(vm);
            return vm;
        }

        private static void InitializeInitialziable(IInitializable initializable)
        {
            initializable?.Init();
        }
    }
}