using System;
using CredentialManagement;

namespace Blue10CLI.services
{
    public class CredentialsService
    {
        internal const string BLUE10_API_KEY_TARGET = "Blue10ApiKey";
        
        public CredentialsService() {}
        
        public string? GetApiKey()
        {
            var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
            cm.Load();
            return cm?.Password;
        }

        public bool SetApiKey(string apiKey) => SetCredential(BLUE10_API_KEY_TARGET, apiKey);
            

        public bool RemoveCredentials()
        {
            var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
            return cm.Delete();
        }

        private static bool SetCredential(string taget, string secret) =>
            new Credential {Target = BLUE10_API_KEY_TARGET, Password = secret, PersistanceType =  PersistanceType.LocalComputer}
            .Save();
        
        internal static string? EnsureApiKey()
        {
            var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
            cm.Load();
            var apiKey = cm?.Password;
            if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;
            Console.WriteLine("Missing Blue10 API key, please insert here:");
            var key =ReadPassword();
            SetCredential(BLUE10_API_KEY_TARGET, key);
            return key;
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
                        int pos = Console.CursorLeft;
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