using System;
using System.IO;
using Microsoft.Extensions.Logging;
using WpfApp.Interfaces;
using WpfApp.Interfaces.Services;

namespace WpfApp.Logic.Services
{
    public class DirectoryService : IDirectoryService
    {
        public string DatabaseFolder { get; }
        public string SettingsFolder { get; }
        public string ApplicationFolder { get; }

        public DirectoryService()
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Constants.ApplicationName);
            ApplicationFolder = basePath;
            
            DatabaseFolder = Path.Combine(basePath, Constants.DatabaseDirectory);
            SettingsFolder = Path.Combine(basePath, Constants.SettingsDirectory);
            LogsFolder = Path.Combine(basePath, Constants.LoggerDirectory);
            CreateDirectoryIfNotExists(DatabaseFolder);
            CreateDirectoryIfNotExists(SettingsFolder);
            CreateDirectoryIfNotExists(LogsFolder);
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public string LogsFolder { get; }
    }
}