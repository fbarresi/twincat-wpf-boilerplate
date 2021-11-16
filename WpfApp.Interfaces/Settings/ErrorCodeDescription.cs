using WpfApp.Interfaces.Enums;

namespace WpfApp.Interfaces.Settings
{
    public class ErrorCodeDescription
    {
        public object Value { get; set; }
        public string Description { get; set; }
        public Severity Severity { get; set; }
        public string LongDescription { get; set; }
    }
}