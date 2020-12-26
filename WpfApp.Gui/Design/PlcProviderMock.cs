using WpfApp.Interfaces.Hardware;
using WpfApp.Interfaces.Services;

namespace WpfApp.Gui.Design
{
    internal class PlcProviderMock : IPlcProvider
    {
        public IPlc GetHardware(string name)
        {
            return new MockPlc();
        }

        public IPlc GetHardware()
        {
            return new MockPlc();
        }
    }
}