using System;
using System.IO;
using System.Text.Json;

namespace Blue10CLI.Helpers
{

    public static class AppConfiguration
    {
        internal const string CONFIG_FILE = "AppConfiguration.json";

        public static Values Values { get; set; } = new Values();

        public static void GetSettings()
        {
            if (File.Exists(CONFIG_FILE))
            {
                try
                {
                    var fJsonFile = File.ReadAllText(CONFIG_FILE);
                    if (string.IsNullOrWhiteSpace(fJsonFile))
                    {
                        Console.WriteLine("AppConfiguration.json file appears to be empty");
                        return;
                    }

                    var fAppConfigration = JsonSerializer.Deserialize<Values>(fJsonFile);

                    if (fAppConfigration != null)
                        Values = fAppConfigration;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading configurations from AppConfiguration.json file: " + e);
                    return;
                }
            }
            else
            {
                var fStringJson = JsonSerializer.Serialize(Values);
                File.WriteAllText(CONFIG_FILE, fStringJson);
            }
        }

        public static bool SaveSettings()
        {
            try
            {
                var fStringJson = JsonSerializer.Serialize(Values);
                File.WriteAllText(CONFIG_FILE, fStringJson);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writng configurations to AppConfiguration.json file: " + e);
                return false;
            }
        }
    }

    public class Values
    {
        internal const string DEFAULT_BASEURL = "https://api.blue10.com/v2/";

        public string? ApiKey { get; set; } = string.Empty;

        private string? baseUrl = DEFAULT_BASEURL;
        public string? BaseUrl
        {
            get { return baseUrl; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    baseUrl = DEFAULT_BASEURL;
                baseUrl = value;
            }
        }
    }
}
