using WpfApp.Interfaces.Settings;

namespace WpfApp.Interfaces.Services
{
    public interface ISettingsProvider
    {
        SettingRoot SettingRoot { get; set; }
        void SaveSettings();
    }
}