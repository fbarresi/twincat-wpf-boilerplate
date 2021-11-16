using System.Collections.Generic;

namespace WpfApp.Interfaces.Settings
{
    public class ErrorSetting
    {
        public List<ErrorCodeSetting> ErrorCodeSettings { get; set; } = new List<ErrorCodeSetting>();
    }
}