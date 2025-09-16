namespace PushPoCApp.Models
{
    public class SubscriptionDto
    {
        public string endpoint { get; set; } = string.Empty;
        public SubKeys keys { get; set; } = new SubKeys();
    }

    public class SubKeys
    {
        public string p256dh { get; set; } = string.Empty;
        public string auth { get; set; } = string.Empty;
    }
}
