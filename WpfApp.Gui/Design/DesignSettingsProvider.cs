using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Gui.Design
{
    internal class DesignSettingsProvider : ISettingsProvider
    {
        public SettingRoot SettingRoot => new SettingRoot();
        public void SaveSettings()
        {
        }
    }
}