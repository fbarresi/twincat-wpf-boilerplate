using WpfApp.Interfaces.Hardware;

namespace WpfApp.Interfaces.Services
{
    public interface IPlcProvider
    {
        IPlc GetHardware(string name);
    }
}