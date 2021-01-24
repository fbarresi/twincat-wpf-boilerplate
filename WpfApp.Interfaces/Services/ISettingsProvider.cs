using WpfApp.Interfaces.Settings;

namespace WpfApp.Interfaces.Services
{
    public interface ISettingsProvider
    {
        SettingRoot SettingRoot { get; }
        void SaveSettings();
    }
}