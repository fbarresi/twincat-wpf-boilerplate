using System;
using Ninject;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;
using IInitializable = Ninject.IInitializable;

namespace WpfApp.Logic.Services
{
    public class SettingsService : IInitializable, ISettingsProvider
    {
        public SettingRoot SettingRoot { get; set; }
        public void Initialize()
        {
            //todo: add serialize and deserialize
            SettingRoot = new SettingRoot();
        }
    }
}