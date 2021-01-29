using Microsoft.Extensions.Logging;

namespace Blue10CLI.services
{
    public class VendorService
    {
        private readonly ILogger<VendorService> _logger;

        public VendorService(ILogger<VendorService> logger)
        {
            _logger = logger;
        }

        public void Create()
        {
            _logger.LogInformation("Created vendor!");
        }
    }
}