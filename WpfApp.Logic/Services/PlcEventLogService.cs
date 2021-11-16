using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;

namespace WpfApp.Logic.Services
{
    public class PlcEventLogService : IPlcEventLogService
    {
        private readonly IDatabaseService databaseService;
        private const string CollectionName = "EventArchive";
        public PlcEventLogService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public void LogEvent(PlcEvent plcEvent)
        {
            databaseService.InsertIntoCollection(CollectionName, plcEvent);
        }
    }
}