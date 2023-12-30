using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Events;

namespace Neo.Infrastructure.Services
{
    public class EventService
    {
        private readonly List<Uri> _relays;
        private readonly string _subscriptionId;
        private readonly object _subscriptionFilter;

        public EventService(string subscriptionId, object subscriptionFilter)
        {
            _subscriptionId = subscriptionId;
            _subscriptionFilter = subscriptionFilter;
            _relays = [
                new Uri("wss://nos.lol"),
                new Uri("wss://nostr.kungfu-g.rip")
            ];
        }

        public async Task<List<NostrEvent>> GetEventsAsync()
        {
            List<NostrEvent> events = [];
            int eoseCount = 0;

            var pool = new Pool(_relays, _subscriptionId, _subscriptionFilter);

            pool.EventsReceived += (sender, e) =>
            {
                events.AddRange(e.events);
            };

            pool.EoseReceived += (_, _) => eoseCount++;

            await pool.ConnectAndSubscribeAsync();

            int timeout = 0;
            while (eoseCount < _relays.Count && timeout <= 30)
            {
                timeout++;
                await Task.Delay(1000);
            }
            await pool.DisconnectAsync();

            return events;
        }


    }
}
