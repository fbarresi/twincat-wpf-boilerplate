﻿using MahApps.Metro.Controls.Dialogs;
using Ninject.Modules;
using WpfApp.Gui.Services;
using WpfApp.Interfaces.Ui;

namespace WpfApp.Gui
{
    public class GuiModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<IPresentationService>().To<PresentationService>().InSingletonScope();
            Bind<IDialogCoordinator>().To<DialogCoordinator>().InSingletonScope();
        }
    }
}