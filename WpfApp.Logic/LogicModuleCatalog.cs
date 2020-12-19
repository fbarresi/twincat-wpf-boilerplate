﻿﻿using System;
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
            
            Bind<HardwareSetting>().ToMethod(context =>
                context.Kernel.Get<ISettingsProvider>().SettingRoot.HardwareSetting);

            Bind<IPlcProvider>().To<PlcProvider>().InSingletonScope();
            Bind<BeckhoffPlc>().ToSelf();
        }
    }
}