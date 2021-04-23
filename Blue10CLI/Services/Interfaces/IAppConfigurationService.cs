using Blue10CLI.Models;

namespace Blue10CLI.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        public Settings GetSettings();

        public void SetSettings(Settings pSettings);

        public void ImportSettings();

        public bool SaveSettings();

    }
}
