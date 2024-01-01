using NostrNetTools.Nostr.Keys;

namespace Neo.Domain.Interfaces
{
    public interface IUserKeyService
    {
        Task SaveKeySet(NostrKeySet nostrKeySet);
        Task<NostrKeySet> GetKeySet();
    }
}
