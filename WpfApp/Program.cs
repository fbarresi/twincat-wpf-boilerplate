using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using Microsoft.VisualBasic;
using Ninject;
using WpfApp.Gui;
using WpfApp.Gui.ViewModels;
using WpfApp.Interfaces;

namespace WpfApp
{
    class Program
    {
		[STAThread]
		public static void Main(string[] args)
		{
			using (IKernel kernel = new StandardKernel())
			{
				try
				{
					CreateLogger();
					// LoggerFactory.GetLogger().Info(string.Join("", Enumerable.Repeat("#",80)));
					// LoggerFactory.GetLogger().Info("Application starts!");
					// LoggerFactory.GetLogger().Info("Loading kernel modules... ");
					LoadModules(kernel);
					// LoggerFactory.GetLogger().Info("Kernel modules loaded");

					var viewModelFactory = kernel.Get<ViewModelLocator>();
					var application = CreateApplication(viewModelFactory);

					var mainWindowViewModel = viewModelFactory.CreateViewModel<MainWindowViewModel>();

					var mainWindow = kernel.Get<MainWindow>();
					mainWindow.DataContext = mainWindowViewModel;

					// LoggerFactory.GetLogger().Info(string.Join("", Enumerable.Repeat("#",80)));

					application.Run(mainWindow);
					application.Shutdown();
					// LoggerFactory.GetLogger().Info("Application ended...");
					// LoggerFactory.GetLogger().Info("\n\n\n\n");
				}
				catch (Exception e)
				{
					// LoggerFactory.GetLogger().Error("Unhandled exception", e);

                    throw e;
				}
			}
		}

		private static void CreateLogger()
		{
			// log4net.Config.XmlConfigurator.Configure(new FileInfo("log.config"));
			// LogManager.CreateRepository(Constants.LoggingRepositoryName);
			// LogManager.CreateRepository(Constants.LoggingObservationRepositoryName);
		}

		private static void LoadModules(IKernel kernel)
		{
			kernel.Load<GuiModuleCatalog>();
			// kernel.Load<LogicModuleCatalog>();
		}

		private static Application CreateApplication(IViewModelFactory viewModelLocator)
		{
			var application = new App() { ShutdownMode = ShutdownMode.OnLastWindowClose };

			application.InitializeComponent();

			return application;
		}
	}

}