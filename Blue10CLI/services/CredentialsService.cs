using Blue10CLI.Helpers;
using System;

namespace Blue10CLI.Services
{
    public class CredentialsService
    {
        internal enum EStorageSolution
        {
            AppConfiguration,
            WindowsCredentialsManagement
        }

        internal static EStorageSolution StorageSolution = EStorageSolution.AppConfiguration;

        public string? GetApiKey()
        {
            var fApiKey = AppConfiguration.Values.ApiKey;

            if (string.IsNullOrWhiteSpace(fApiKey))
                return null;

            return fApiKey;
        }

        public bool SetApiKey(string apiKey)
        {
            AppConfiguration.Values.ApiKey = apiKey;
            return AppConfiguration.SaveSettings();
        }

        public bool RemoveCredentials()
        {
            AppConfiguration.Values.ApiKey = null;
            return AppConfiguration.SaveSettings();
        }


        internal static string? EnsureApiKey()
        {
            if (!string.IsNullOrEmpty(AppConfiguration.Values.ApiKey))
                return AppConfiguration.Values.ApiKey;

            Console.WriteLine("Missing Blue10 API key, please insert here:");
            var fApiKey = ReadPassword();

            AppConfiguration.Values.ApiKey = fApiKey;
            if (!AppConfiguration.SaveSettings())
                Console.WriteLine("Something went wrong saving API Key to AppConfiguration.json");

            Console.WriteLine("API Key has been saved to AppConfiguration.json");

            return fApiKey;
        }

        public static string ReadPassword()
        {
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