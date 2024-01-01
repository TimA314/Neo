using Neo.Domain.Interfaces;
using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Events;

namespace Neo.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly List<Uri> _relays;
        private Dictionary<string, NostrEvent> _profileCache = [];
        private Pool _pool;


        public EventService()
        {
            _relays = [
                new Uri("wss://nos.lol"),
                new Uri("wss://nostr.kungfu-g.rip"),
                new Uri("wss://purplepag.es"),
                new Uri("wss://relay.nostr.band"),
            ];

            _pool = new Pool(_relays);
        }

        public async Task<List<NostrEvent>> ListEvents(string subscriptionId, object filter)
        {
            List<NostrEvent> events = [];
            int eoseCount = 0;

            try
            {

                _pool.EventsReceived += (sender, e) =>
                {
                    events.AddRange(e.events);
                };

                _pool.EoseReceived += (_, _) => eoseCount++;

                await _pool.ConnectAsync();
                await _pool.SubscribeAsync(subscriptionId, filter);

                int timeout = 0;
                while (eoseCount < _relays.Count && timeout <= 30)
                {
                    timeout++;
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return events;
        }

        public async Task SubscribeToEvents(Action<NostrEvent> onEventReceived, string subscriptionId, object subscriptionFilter)
        {
            try
            {
                int eoseCount = 0;

                _pool.EventsReceived += (sender, e) =>
                {
                    foreach (var evt in e.events)
                    {
                        onEventReceived?.Invoke(evt);
                    }
                };

                _pool.EoseReceived += (_, _) => eoseCount++;

                await _pool.ConnectAsync();
                await _pool.SubscribeAsync(subscriptionId, subscriptionFilter);

                int timeout = 0;
                while (eoseCount < _relays.Count && timeout <= 30)
                {
                    timeout++;
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<Dictionary<string, NostrEvent>> GetProfileDataAsync(List<string> publicKeys)
        {
            Dictionary<string, NostrEvent> profiles = [];

            // Fetch profiles that are not in the cache
            var keysToFetch = publicKeys.Where(pk => !_profileCache.ContainsKey(pk)).ToList();
            if (keysToFetch.Count != 0)
            {
                var profileFilter = new { kinds = new[] { 0 }, authors = keysToFetch.ToArray() };
                var profileEvents = await ListEvents("profile-subscription", profileFilter);

                foreach (var profileEvent in profileEvents)
                {
                    if (profileEvent != null && !string.IsNullOrEmpty(profileEvent.PublicKey))
                    {
                        _profileCache[profileEvent.PublicKey] = profileEvent;
                    }
                }
            }

            // Add profiles from cache to return dictionary
            foreach (var key in publicKeys)
            {
                if (_profileCache.TryGetValue(key, out var profile))
                {
                    profiles[key] = profile;
                }
            }

            return profiles;
        }
    }
}
