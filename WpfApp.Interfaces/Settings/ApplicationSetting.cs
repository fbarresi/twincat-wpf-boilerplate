using System;

namespace WpfApp.Interfaces.Settings
{
    public class ApplicationSetting
    {
        public string ToggleSignalName { get; set; } = "Utils.Toggle";
        public string DoubleSignalName { get; set; } = "Utils.Random";
        public TimeSpan Autologout { get; set; } = TimeSpan.FromMinutes(5);
    }
}