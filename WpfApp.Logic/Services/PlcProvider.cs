using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Parameters;
using WpfApp.Interfaces.Commons;
using WpfApp.Interfaces.Hardware;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;
using WpfApp.Logic.Hardware;
using IInitializable = Ninject.IInitializable;

namespace WpfApp.Logic.Services
{
    public class PlcProvider : IPlcProvider, IInitializable
    {
        private readonly HardwareSetting setting;
        private readonly IInstanceCreator instanceCreator;

        private readonly Dictionary<string, IPlc> plcs = new Dictionary<string, IPlc>();

        [Inject]
        private ILogger Logger { get; set; }

        public PlcProvider(HardwareSetting setting, IInstanceCreator instanceCreator)
        {
            this.setting = setting;
            this.instanceCreator = instanceCreator;
        }

        public IPlc GetHardware(string name)
        {
            if (plcs.ContainsKey(name))
            {
                return plcs[name];
            }

            return null;
        }

        public void Initialize()
        {
            foreach (var plcSetting in setting.PlcSettings)
            {
                if (!plcSetting.IsMock)
                {
                    var plc = instanceCreator.CreateInstance<BeckhoffPlc>(new[]
                        {new ConstructorArgument("settings", plcSetting)});
                    if (plc.Initialize())
                        plcs.Add(plcSetting.Name, plc);
                    else
                    {
                        Logger?.LogWarning("Unable to initialize Beckhoff: '{plcSetting}'", plcSetting);
                    }
                }
                else
                {
                    plcs.Add(plcSetting.Name, new MockPlc());
                }
            }
        }
    }
}