//using Microsoft.Extensions.Logging;
//using System;

//namespace Blue10CLI.Services
//{
//    public class WindowsCredentialService
//    {
//        private readonly ILogger<WindowsCredentialService> _log;
//        internal const string BLUE10_API_KEY_TARGET = "Blue10ApiKey";

//        public string? GetApiKey()
//        {
//            try
//            {
//                //StorageSolution = EStorageSolution.WindowsCredentialsManagement;
//                // var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
//                // cm.Load();
//                // cm?.Password;
//                return null;
//            }
//            catch (Exception e)
//            {
//                _log.LogError("Error retrieving apikey from Windows credentials manager", e);
//                return null;
//            }
//        }

//        public bool SetApiKey(string apiKey)
//        {
//            try
//            {
//                return SaveCredentialsInWindowsCredentialsManager(BLUE10_API_KEY_TARGET, apiKey);
//            }
//            catch (Exception e)
//            {
//                _log.LogError("Error writing api-key to Windows credential management", e);
//                return false;
//            }
//        }

//        public bool RemoveCredentials()
//        {
//            try
//            {
//                //var cm = new Credential {Target = BLUE10_API_KEY_TARGET};
//                //return cm.Delete();
//                return false;
//            }
//            catch (Exception e)
//            {
//                _log.LogError("Error removing credentials from windows credentials manager", e);
//                return false;
//            }
//        }

//        private static bool SaveCredentialsInWindowsCredentialsManager(string taget, string secret) => false;//new Credential {Target = BLUE10_API_KEY_TARGET, Password = secret, PersistanceType = PersistanceType.LocalComputer}.Save();

//        internal static string? EnsureApiKey()
//        {


//            Console.WriteLine("No Authentication file found, using Windows credentials management to store credentials");
//            try
//            {
//                var cm = BLUE10_API_KEY_TARGET;// new Credential {Target = BLUE10_API_KEY_TARGET};
//                                               // cm.Load();   
//                var apiKey = BLUE10_API_KEY_TARGET;
//                if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;
//                Console.WriteLine("Missing Blue10 API key, please insert here:");
//                var key = CredentialsService.ReadPassword();
//                SaveCredentialsInWindowsCredentialsManager(BLUE10_API_KEY_TARGET, key);
//                return key;
//            }
//            catch (Exception e)
//            {
//                //pass
//                Console.WriteLine("Could not initiate windows credentials manager");
//                return null;
//            }

//        }
//    }
//}
