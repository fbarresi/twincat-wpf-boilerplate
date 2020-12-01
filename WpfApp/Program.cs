using System;
using System.IO;
using System.Linq;
using System.Windows;
using Ninject;
using Serilog;
using WpfApp.Gui;
using WpfApp.Gui.ViewModels;
using WpfApp.Gui.Views;
using WpfApp.Interfaces;
using WpfApp.Interfaces.Commons;
using WpfApp.Logic;

namespace WpfApp
{
	static class Program
    {
		[STAThread]
		public static void Main(string[] args)
		{
			using (IKernel kernel = new StandardKernel())
			{
				var logger = CreateLogger();
				try
				{
					logger.Information(string.Join("", Enumerable.Repeat("#",80)));
					logger.Information("Application starts!");
					logger.Information("Loading kernel modules... ");
					LoadModules(kernel);
					logger.Information("Kernel modules loaded");

					var viewModelFactory = kernel.Get<ViewModelLocator>();
					var application = CreateApplication(viewModelFactory);

					var mainWindowViewModel = viewModelFactory.CreateViewModel<MainWindowViewModel>();

					var mainWindow = kernel.Get<MainWindow>();
					mainWindow.DataContext = mainWindowViewModel;

					logger.Information(string.Join("", Enumerable.Repeat("#",80)));

					application.Run(mainWindow);
					application.Shutdown();
					logger.Information("Application ended...");
					logger.Information("\n\n\n\n");
				}
				catch (Exception e)
				{
					logger.Error("Unhandled exception: {@Error}", e);

                    throw e;
				}
			}
		}

		private static ILogger CreateLogger()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.File( Path.Combine(Directory.Exists(Constants.DefaultLoggerDirectory) ? Constants.DefaultLoggerDirectory : "", "WpfApp.log"), 
					rollOnFileSizeLimit: true, 
					retainedFileCountLimit: 10, 
					fileSizeLimitBytes: 102400)
				.CreateLogger();
			
			return Log.Logger;
		}

		private static void LoadModules(IKernel kernel)
		{
			kernel.Load<GuiModuleCatalog>();
			kernel.Load<LogicModuleCatalog>();
			kernel.Bind<ILogger>().ToConstant(Log.Logger);
		}

		private static Application CreateApplication(IViewModelFactory viewModelLocator)
		{
			var application = new App() { ShutdownMode = ShutdownMode.OnLastWindowClose };

			application.InitializeComponent();
			application.ReplaceViewModelLocator(viewModelLocator);

			return application;
		}
	}
    
    public static class ApplicationExtensions
    {
	    public static void ReplaceViewModelLocator(this Application application, IViewModelFactory viewModelLocator, string locatorKey = "Locator")
	    {
		    if (application.Resources.Contains(locatorKey))
		    {
			    application.Resources.Remove(locatorKey);
		    }

		    application.Resources.Add(locatorKey, viewModelLocator);
	    }
    }
}