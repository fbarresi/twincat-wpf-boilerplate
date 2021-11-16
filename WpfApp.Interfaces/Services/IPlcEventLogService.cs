using WpfApp.Interfaces.Models;

namespace WpfApp.Interfaces.Services
{
    public interface IPlcEventLogService
    {
        void LogEvent(PlcEvent plcEvent);
        PlcEvent[] GetHistory(int elements, int page);
    }
}