using System;
 using Ninject;
 using Ninject.Modules;
 using Serilog;
 using WpfApp.Interfaces.Services;
 using WpfApp.Interfaces.Settings;
 using WpfApp.Logic.Hardware;
 using WpfApp.Logic.Services;

 namespace WpfApp.Logic
{
    public class LogicModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<ISettingsProvider>().To<SettingsService>().InSingletonScope();
            Bind<IDirectoryService>().To<DirectoryService>().InSingletonScope();
            Bind<IDatabaseService>().To<DatabaseService>().InSingletonScope();
            
            Bind<ApplicationSetting>().ToMethod(context =>
                context.Kernel.Get<ISettingsProvider>().SettingRoot.ApplicationSetting);
            
            Bind<SettingRoot>().ToMethod(context =>
                context.Kernel.Get<ISettingsProvider>().SettingRoot);

            Bind<HardwareSetting>().ToMethod(context =>
                context.Kernel.Get<ISettingsProvider>().SettingRoot.HardwareSetting);
            
            Bind<ErrorSetting>().ToMethod(context =>
                context.Kernel.Get<ISettingsProvider>().SettingRoot.ErrorSetting);

            Bind<IPlcProvider>().To<PlcProvider>().InSingletonScope();
            Bind<BeckhoffPlc>().ToSelf();

            Bind<IUserService>().To<UserService>().InSingletonScope();
            Bind<IPlcEventLogService>().To<PlcEventLogService>().InSingletonScope();
            Bind<IPlcEventService>().To<PlcEventService>().InSingletonScope();
        }
    }
}