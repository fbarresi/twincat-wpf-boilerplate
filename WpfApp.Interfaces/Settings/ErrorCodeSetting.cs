using System.Collections.Generic;
using WpfApp.Interfaces.Enums;

namespace WpfApp.Interfaces.Settings
{
    public class ErrorCodeSetting
    {
        public string PlcName { get; set; }
        public string ErrorCodeAddress { get; set; }
        public List<ErrorCodeDescription> CodeDescriptions { get; set; }
        public bool IgnoreNotDescribedValues { get; set; }
        public object NoErrorValue { get; set; }
        public Severity DefaultSeverity { get; set; } = Severity.Warning;
    }
}