namespace WpfApp.Interfaces.Services
{
    public interface IDirectoryService
    {
        string DatabaseFolder { get; }
        string SettingsFolder { get; }
        string LogsFolder { get; }
        string ApplicationFolder { get; }
    }
}