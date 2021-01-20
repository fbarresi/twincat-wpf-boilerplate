using System.Globalization;

namespace WpfApp.Interfaces.Settings
{
    public class CultureSetting
    {
        public CultureInfo SelectedCulture { get; set; } = CultureInfo.GetCultureInfo("en");

        public CultureInfo[] SupportedCultures =>
            new[] {CultureInfo.GetCultureInfo("en"), CultureInfo.GetCultureInfo("de")};
    }
}