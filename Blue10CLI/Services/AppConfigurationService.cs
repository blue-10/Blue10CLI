using Blue10CLI.Models;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace Blue10CLI.Services
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly ILogger<AppConfigurationService> _logger;

        private const string CONFIG_FILE = "AppConfiguration.json";

        private Settings AppSettings = new Settings();

        public AppConfigurationService(ILogger<AppConfigurationService> logger)
        {
            _logger = logger;
        }

        public Settings GetSettings()
        {
            return AppSettings;
        }

        public void SetSettings(Settings pSettings)
        {
            AppSettings = pSettings;
        }

        public void ImportSettings()
        {
            if (File.Exists(CONFIG_FILE))
            {
                try
                {
                    var fJsonFile = File.ReadAllText(CONFIG_FILE);
                    if (string.IsNullOrWhiteSpace(fJsonFile))
                    {
                        _logger.LogError("AppConfiguration.json file is empty");
                        return;
                    }

                    var fAppConfigration = JsonSerializer.Deserialize<Settings>(fJsonFile);

                    if (fAppConfigration != null)
                    {
                        AppSettings = fAppConfigration;
                    }
                }
                catch (JsonException e)
                {
                    _logger.LogError($"Error reading configurations from {CONFIG_FILE} file: {e}");
                    return;
                }
            }
            else
            {
                var fStringJson = JsonSerializer.Serialize(AppSettings);
                File.WriteAllText(CONFIG_FILE, fStringJson);
            }
        }

        public bool SaveSettings()
        {
            try
            {
                var fStringJson = JsonSerializer.Serialize(AppSettings);
                File.WriteAllText(CONFIG_FILE, fStringJson);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error writng configurations to {CONFIG_FILE} file: {e}");
                return false;
            }
        }
    }
}
