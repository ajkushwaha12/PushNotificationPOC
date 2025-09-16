using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushPoCApp.Models;
using PushPoCApp.Services;
using System.Net;
using WebPush;

namespace PushPoCApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PushController : ControllerBase
    {
        private readonly ILogger<PushController> _logger;
        private readonly VapidService _vapid;
        private readonly SubscriptionStore _store;

        public PushController(ILogger<PushController> logger, VapidService vapid, SubscriptionStore store)
        {
            _logger = logger;
            _vapid = vapid;
            _store = store;
        }

        [HttpGet("vapidPublicKey")]
        public IActionResult GetPublicKey()
        {
            _logger.LogInformation("Returning VAPID public key to client");
            return Ok(_vapid.Keys.PublicKey);
        }

        [HttpPost("subscribe")]
        public IActionResult Subscribe([FromBody] SubscriptionDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.endpoint))
            {
                _logger.LogWarning("Invalid subscription received");
                return BadRequest(new { success = false, error = "Invalid subscription" });
            }

            var sub = new PushSubscription(dto.endpoint, dto.keys.p256dh, dto.keys.auth);
            _store.Add(sub);
            _logger.LogInformation("Subscription added: {endpoint}", Truncate(dto.endpoint, 60));
            _logger.LogDebug("Subscription keys: p256dh({p}), auth({a})", Truncate(dto.keys.p256dh, 10), Truncate(dto.keys.auth, 6));
            return Ok(new { success = true });
        }

        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe([FromBody] SubscriptionDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.endpoint))
            {
                _logger.LogWarning("Invalid unsubscribe request");
                return BadRequest(new { success = false, error = "Invalid request" });
            }

            var removed = _store.Remove(dto.endpoint);
            _logger.LogInformation("Unsubscribe request for {endpoint}, removed: {removed}", Truncate(dto.endpoint, 60), removed);
            return Ok(new { success = true, removed });
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationPayload payload)
        {
            payload ??= new NotificationPayload();
            _logger.LogInformation("Sending notification to all subscriptions. Title={title} Body={body}", payload.Title, payload.Body);

            var vapidDetails = new VapidDetails("mailto:ajkushwawaha12@gmail.com", _vapid.Keys.PublicKey, _vapid.Keys.PrivateKey);
            var webPushClient = new WebPushClient();
            var results = new List<object>();

            foreach (var sub in _store.GetAll())
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        title = payload.Title,
                        body = payload.Body,
                        icon = payload.Icon,
                        url = payload.Url
                    });

                    _logger.LogDebug("Sending to {endpoint} payload: {json}", Truncate(sub.Endpoint, 60), json);
                    await webPushClient.SendNotificationAsync(sub, json, vapidDetails);

                    _logger.LogInformation("Sent to {endpoint}", Truncate(sub.Endpoint, 60));
                    results.Add(new { endpoint = sub.Endpoint, status = "sent" });
                }
                catch (WebPushException wex)
                {
                    _logger.LogError(wex, "Error sending to {endpoint} - StatusCode: {code}", Truncate(sub.Endpoint, 60), wex.StatusCode);
                    if (wex.StatusCode == HttpStatusCode.Gone || wex.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogInformation("Subscription seems invalid - removing {endpoint}", Truncate(sub.Endpoint, 60));
                        _store.Remove(sub.Endpoint);
                    }
                    results.Add(new { endpoint = sub.Endpoint, status = "error", code = (int?)wex.StatusCode, message = wex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unknown error sending to {endpoint}", Truncate(sub.Endpoint, 60));
                    results.Add(new { endpoint = sub.Endpoint, status = "error", message = ex.Message });
                }
            }

            return Ok(new { success = true, results });
        }

        private static string Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}
