using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace Blue10CLI.Services
{
    public class CredentialsService : ICredentialsService
    {
        private readonly IAppConfigurationService _appConfiguration;
        private readonly ILogger<CredentialsService> _logger;

        public CredentialsService(IAppConfigurationService appConfiguration, ILogger<CredentialsService> logger)
        {
            _appConfiguration = appConfiguration;
            _logger = logger;
        }

        public string? GetApiKey()
        {
            var fApiKey = _appConfiguration.GetSettings().ApiKey;

            if (string.IsNullOrWhiteSpace(fApiKey))
                return null;

            return fApiKey;
        }

        public bool SetApiKey(string apiKey)
        {
            var fSettings = _appConfiguration.GetSettings();
            fSettings.ApiKey = apiKey;
            _appConfiguration.SetSettings(fSettings);
            return _appConfiguration.SaveSettings();
        }

        public bool RemoveCredentials()
        {
            var fSettings = _appConfiguration.GetSettings();
            fSettings.ApiKey = null;
            _appConfiguration.SetSettings(fSettings);
            return _appConfiguration.SaveSettings();
        }

        public string? EnsureApiKey()
        {
            var fSettings = _appConfiguration.GetSettings();
            if (!string.IsNullOrEmpty(fSettings.ApiKey))
                return fSettings.ApiKey;

            _logger.LogWarning("Missing Blue10 API key.");

            var fApiKey = ReadPassword();

            fSettings.ApiKey = fApiKey;
            _appConfiguration.SetSettings(fSettings);

            if (!_appConfiguration.SaveSettings())
                _logger.LogError("Something went wrong saving API Key to AppConfiguration.json");

            Console.WriteLine("API Key has been saved to AppConfiguration.json");

            return fApiKey;
        }

        public string ReadPassword()
        {
            Console.WriteLine("Missing Blue10 API key, please insert here:");

            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        var pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
    }
}