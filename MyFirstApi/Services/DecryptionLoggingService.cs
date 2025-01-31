using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class DecryptionLoggingService
    {

        private readonly ILogger<DecryptionLoggingService> logger;

        public DecryptionLoggingService(ILogger<DecryptionLoggingService> logger)
        {
            this.logger = logger;
        }

        public void OnValueDecrypted(string cipherText, string plaintext)
        {
            logger.LogInformation($"[Logging] Decrypted: {cipherText} to {plaintext}");
        }
    }
}