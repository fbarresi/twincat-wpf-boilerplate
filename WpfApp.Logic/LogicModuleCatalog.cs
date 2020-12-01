﻿using System;
using Ninject.Modules;
 using WpfApp.Interfaces.Services;
 using WpfApp.Logic.Services;

 namespace WpfApp.Logic
{
    public class LogicModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<ISettingsProvider>().To<SettingsService>().InSingletonScope();
        }
    }
}