namespace WpfApp.Interfaces.Settings
{
    public class SettingRoot
    {
        public ApplicationSetting ApplicationSetting { get; set; } = new ApplicationSetting();
        public HardwareSetting HardwareSetting { get; set; } = new HardwareSetting();
    }
}