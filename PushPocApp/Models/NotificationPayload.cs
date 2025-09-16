namespace PushPoCApp.Models
{
    public class NotificationPayload
    {
        public string Title { get; set; } = "Notification Title";
        public string Body { get; set; } = "Hello from server";
        public string Icon { get; set; } = "/icon.png";
        public string Url { get; set; } = "/";
    }
}
