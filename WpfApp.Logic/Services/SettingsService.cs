using System;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Ninject;
using Serilog;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;
using IInitializable = Ninject.IInitializable;

namespace WpfApp.Logic.Services
{
    public class SettingsService : IInitializable, ISettingsProvider, IDisposable
    {
        private readonly IDirectoryService directoryService;
        
        [Inject]
        public ILogger Logger { get; set; }

        public SettingsService(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }
        
        public SettingRoot SettingRoot { get; set; }
        public void Initialize()
        {
            //serialize default settings
            var defaultSettings = new SettingRoot();
            defaultSettings.CreateDafaultSettings();
            var defaultSettingsJson = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(Path.Combine(directoryService.SettingsFolder, "default_settings.json"), defaultSettingsJson);

            //deserialize settings or get default
            try
            {
                var settingsFile = Path.Combine(directoryService.SettingsFolder, "settings.json");
                if (File.Exists(settingsFile))
                {
                    Logger?.Information($"Deserializing settings from file {settingsFile}");
                    SettingRoot = JsonConvert.DeserializeObject<SettingRoot>(File.ReadAllText(settingsFile));
                }
                else
                {
                    Logger?.Information("Creating settings from default because there was no settings file");
                    SettingRoot = defaultSettings;
                    Logger?.Information("Creating settings file");
                    File.WriteAllText(Path.Combine(directoryService.SettingsFolder, "settings.json"), defaultSettingsJson);
                }

            }
            catch (Exception e)
            {
                Logger?.Error(e, "Error while deserializing settings");
                Logger?.Information("Creating settings from default");
                SettingRoot = defaultSettings;
            }
        }

        public void Dispose()
        {
            Logger?.Information("Saving application settings on dispose");
            File.WriteAllText(Path.Combine(directoryService.SettingsFolder, "settings.json"),
                JsonConvert.SerializeObject(SettingRoot, Formatting.Indented));

        }
    }
}