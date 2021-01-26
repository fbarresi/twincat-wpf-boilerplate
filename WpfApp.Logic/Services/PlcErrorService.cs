using Ninject;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Logic.Services
{
    public class PlcErrorService : IPlcErrorService
    {
        private readonly ApplicationSetting setting;
        private readonly IPlcProvider plcProvider;
        private readonly IDatabaseService databaseService;
        private const string CollectionName = "ErrorArchive";
        public PlcErrorService(ApplicationSetting setting, 
            IPlcProvider plcProvider, 
            IDatabaseService databaseService)
        {
            this.setting = setting;
            this.plcProvider = plcProvider;
            this.databaseService = databaseService;
        }

        private void LogError(Error error)
        {
            databaseService.InsertIntoCollection(CollectionName, error);
        }

        
    }
}