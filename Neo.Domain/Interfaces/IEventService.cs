using NostrNetTools.Nostr.Events;

namespace Neo.Domain.Interfaces
{
    public interface IEventService
    {
        Task<List<NostrEvent>> ListEvents(string subscriptionId, object filter);
        Task SubscribeToEvents(Action<NostrEvent> onEventReceived, string subscriptionId, object subscriptionFilter);
        Task<Dictionary<string, NostrEvent>> GetProfileDataAsync(List<string> publicKeys);
    }
}
