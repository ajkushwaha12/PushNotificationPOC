using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebPush;

namespace PushPoCApp.Services
{
    public class VapidKeysDto
    {
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
    }

    public class VapidService
    {
        private readonly ILogger<VapidService> _logger;
        private const string VapidFile = "vapid.json";
        public VapidKeysDto Keys { get; }

        public VapidService(ILogger<VapidService> logger)
        {
            _logger = logger;
            if (!File.Exists(VapidFile))
            {
                _logger.LogInformation("VAPID file not found. Generating new VAPID keys...");
                var generated = VapidHelper.GenerateVapidKeys();
                Keys = new VapidKeysDto { PublicKey = generated.PublicKey, PrivateKey = generated.PrivateKey };
                File.WriteAllText(VapidFile, JsonSerializer.Serialize(Keys));
                _logger.LogInformation("Generated VAPID keys and saved to {file}", VapidFile);
            }
            else
            {
                _logger.LogInformation("Loading existing VAPID keys from {file}", VapidFile);
                var json = File.ReadAllText(VapidFile);
                Keys = JsonSerializer.Deserialize<VapidKeysDto>(json) ?? new VapidKeysDto();
            }

            _logger.LogDebug("VAPID PublicKey (short): {pub}", Keys.PublicKey?.Substring(0, Math.Min(20, Keys.PublicKey.Length)));
        }
    }
}
