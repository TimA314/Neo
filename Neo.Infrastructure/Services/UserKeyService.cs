using Neo.Domain.Interfaces;
using NostrNetTools.Nostr.Keys;
using System.Text.Json;

namespace Neo.Infrastructure.Services
{
    public class UserKeyService : IUserKeyService
    {
        private NostrKeySet _userKeySet;

        public async Task SaveKeySet(NostrKeySet nostrKeySet)
        {
            _userKeySet = nostrKeySet;
            await SecureStorage.SetAsync("keyset", JsonSerializer.Serialize(nostrKeySet));
        }

        public async Task<NostrKeySet> GetKeySet()
        {
            if (_userKeySet is not null)
            {
                return _userKeySet;
            }

            var keySet = await SecureStorage.GetAsync("keyset");

            if (string.IsNullOrEmpty(keySet))
            {
                return null;
            }

            _userKeySet = JsonSerializer.Deserialize<NostrKeySet>(keySet);
            return _userKeySet;
        }
    }
}
