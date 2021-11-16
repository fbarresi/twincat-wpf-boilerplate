using System;
using WpfApp.Interfaces.Enums;

namespace WpfApp.Interfaces.Models
{
    public class PlcEvent : DatabaseObjectBase
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public Severity Severity { get; set; }
        public object Value { get; set; }
        public object ExpectedValue { get; set; }
        public string Source { get; set; }
    }
}