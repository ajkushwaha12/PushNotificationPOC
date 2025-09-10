using System.Collections.Concurrent;
using WebPush;

namespace PushPoCApp.Services
{
    public class SubscriptionStore
    {
        // key by endpoint
        private readonly ConcurrentDictionary<string, PushSubscription> _dict = new();

        public void Add(PushSubscription sub)
        {
            _dict[sub.Endpoint] = sub;
        }

        public bool Remove(string endpoint)
        {
            return _dict.TryRemove(endpoint, out _);
        }

        public List<PushSubscription> GetAll() => _dict.Values.ToList();

        public bool Exists(string endpoint) => _dict.ContainsKey(endpoint);
    }
}
