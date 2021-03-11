using System;
using System.IO;
using CredentialManagement;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.services
{
    public class CredentialsService
    {
        private readonly ILogger<CredentialsService> _log;
        internal const string BLUE10_API_KEY_TARGET = "Blue10ApiKey";

        internal enum EStorageSolution
        {
            AuthFile,
            WindowsCredentialsManagement
        }

        internal static EStorageSolution StorageSolution;
        internal static string ApiKey { get; set; }
        internal const string AUTH_FILE = "api_key.auth";

        public CredentialsService(ILogger<CredentialsService> log)
        {
            _log = log;
        }

        public string? GetApiKey()
        {
            if (File.Exists(AUTH_FILE))
            {
                try
                {
                    ApiKey = File.ReadAllText(AUTH_FILE);
                    StorageSolution = EStorageSolution.AuthFile;
                    return ApiKey;
                }
                catch(Exception e)
                {
                    _log.LogError("Error writing reading apikey from Auth file",e);
                    return null;
                }
            }
            else
            {
                try
                {
                    StorageSolution = EStorageSolution.WindowsCredentialsManagement;
                    var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
                    cm.Load();
                    return cm?.Password;
                }
                catch(Exception e)
                {
                    _log.LogError("Error retrieving apikey from Windows credentials manager",e);
                    return null;
                }
               
            }
        }

        public bool SetApiKey(string apiKey)
        {
            if (File.Exists(AUTH_FILE))
            {
                try
                {
                    File.WriteAllText(AUTH_FILE, apiKey);
                    return true;
                }
                catch(Exception e)
                {
                    _log.LogError("Error writing api-key to file",e);
                    return false;
                }
            }
            else
            {
                try
                {
                    return SaveCredentialsInWindowsCredentialsManager(BLUE10_API_KEY_TARGET, apiKey);
                }
                catch(Exception e)
                {
                    _log.LogError("Error writing api-key to Windows credential management",e);
                    return false;
                }
            }
        } 
            
        public bool RemoveCredentials()
        {
            if (File.Exists(AUTH_FILE))
            {
                try
                {
                    File.WriteAllText(AUTH_FILE, string.Empty);
                    return true;
                }
                catch (Exception e)
                {
                    _log.LogError("Error removing credentials from auth-file", e);
                    return false;
                }
            }
            else
            {
                try
                {
                    var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
                    return cm.Delete();
                }
                catch (Exception e)
                {
                    _log.LogError("Error removing credentials from windows credentials manager", e);
                    return false;
                }
            }
        } 

        private static bool SaveCredentialsInWindowsCredentialsManager(string taget, string secret) =>
            new Credential
                    {Target = BLUE10_API_KEY_TARGET, Password = secret, PersistanceType = PersistanceType.LocalComputer}
                .Save();

        internal static string? EnsureApiKey()
        {
            if (File.Exists(AUTH_FILE))
            {
                try
                {
                    ApiKey = File.ReadAllText(AUTH_FILE);
                    StorageSolution = EStorageSolution.AuthFile;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not read AuthFile");
                    return null;
                }
                
                if (string.IsNullOrWhiteSpace(ApiKey))
                { 
                    Console.WriteLine("Found Auth file but it was empty");
                    return null;
                }
                else
                {
                    return ApiKey;
                }
            }
            else
            {
                Console.WriteLine("No Authentication file found, using Windows credentials management to store credentials");
                try
                {
                    var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
                    cm.Load();
                    var apiKey = cm?.Password;
                    if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;
                    Console.WriteLine("Missing Blue10 API key, please insert here:");
                    var key = ReadPassword();
                    SaveCredentialsInWindowsCredentialsManager(BLUE10_API_KEY_TARGET, key);
                    return key;
                }
                catch (Exception e)
                {
                    //pass
                    Console.WriteLine("Could not initiate windows credentials manager");
                    return null;
                }
            }
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