using System.Collections.Generic;

namespace WpfApp.Interfaces.Settings
{
    public class SettingRoot
    {
        public ApplicationSetting ApplicationSetting { get; set; } = new ApplicationSetting();
        public HardwareSetting HardwareSetting { get; set; } = new HardwareSetting();
        public CultureSetting CultureSettings { get; set; } = new CultureSetting();

        public void CreateDafaultSettings() // Default settings
        {
            ApplicationSetting = new ApplicationSetting();
            HardwareSetting = new HardwareSetting(){PlcSettings = new List<PlcSetting>(){new PlcSetting{IsMock = true}} };
        }
    }
}