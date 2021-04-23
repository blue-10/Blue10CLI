namespace Blue10CLI.Services.Interfaces
{
    public interface ICredentialsService
    {
        public string? GetApiKey();

        public bool SetApiKey(string apiKey);

        public bool RemoveCredentials();

        public string? EnsureApiKey();

        public string ReadPassword();
    }
}