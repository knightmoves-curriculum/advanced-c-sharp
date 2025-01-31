using MyFirstApi.Models;

namespace MyFirstApi.Services
{
    public class DecryptionAuditService
    {
        private readonly ILogger<DecryptionAuditService> logger;

        public DecryptionAuditService(ILogger<DecryptionAuditService> logger)
        {
            this.logger = logger;
        }
        
        public void OnValueDecrypted(string cipherText, string plaintext)
        {
            logger.LogInformation($"[Audit] Decrypted: {cipherText} to {plaintext}");
        }
    }
}