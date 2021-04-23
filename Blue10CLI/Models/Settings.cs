namespace Blue10CLI.Models
{
    public class Settings
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
