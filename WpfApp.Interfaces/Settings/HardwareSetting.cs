using System.Collections.Generic;

namespace WpfApp.Interfaces.Settings
{
    public class HardwareSetting
    {
        public List<PlcSetting> PlcSettings { get; set; } = new List<PlcSetting>(){new PlcSetting(){Port = 851}};
    }
}