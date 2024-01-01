using Neo.Domain.Interfaces;
using Neo.Models;
using Neo.Utilities;
using NostrNetTools.Nostr.Keys;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Neo.ViewModels
{
    public class EventsViewModel
    {
        private readonly IEventService _eventService;
        private bool _isFetchingProfiles;
        private List<Note> _noteBatch = [];
        private readonly NostrKeyService _nostrKeyService = new();
        private readonly object _kind1Filter = new
        {
            kinds = new[] { 1 },
            limit = 100,
        };

        public ObservableCollection<Note> Notes { get; set; }

        public EventsViewModel(IEventService eventService)
        {
            _eventService = eventService;
            Notes = new ObservableCollection<Note>();
            LoadNotes();
        }

        public static string GetTimeAgo(DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            var timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan?.TotalSeconds < 5)
                return "just now";
            if (timeSpan?.TotalSeconds < 60)
                return $"{(int)timeSpan?.TotalSeconds} seconds ago";
            if (timeSpan?.TotalMinutes < 60)
                return $"{(int)timeSpan?.TotalMinutes} minutes ago";
            if (timeSpan?.TotalHours < 24)
                return $"{(int)timeSpan?.TotalHours} hours ago";
            return $"{(int)timeSpan?.TotalDays} days ago";
        }

        private void LoadNotes()
        {
            int BatchSize = 25;

            _ = _eventService.SubscribeToEvents((newEvent =>
            {
                _noteBatch.Add(new Note
                {
                    Id = newEvent.Id,
                    AuthorPublicKey = newEvent.PublicKey,
                    AuthorNpub = _nostrKeyService.ConvertBech32ToNpub(newEvent.PublicKey),
                    ShortAuthorNpub = ShortenPublicKey(_nostrKeyService.ConvertBech32ToNpub(newEvent.PublicKey)),
                    CreatedAt = GetTimeAgo(newEvent.CreatedAt),
                    Content = newEvent.Content,
                });

                if (_noteBatch.Count >= BatchSize || _noteBatch.Count >= 200)
                {
                    BatchSize += 25;
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await FetchAndUpdateProfiles();
                    });
                }
            }), "main-subscription", _kind1Filter);
        }

        private async Task FetchAndUpdateProfiles()
        {
            if (_isFetchingProfiles) return;
            _isFetchingProfiles = true;
            List<string> pubkeys = _noteBatch.Select(b => b.AuthorPublicKey).ToList();
            var profiles = await _eventService.GetProfileDataAsync(pubkeys);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var note in _noteBatch)
                {
                    try
                    {
                        var profile = profiles[note.AuthorPublicKey];
                        if (profile is null) continue;
                        var unescapedContent = Regex.Unescape(profile.Content);

                        var profileContent = JsonSerializer.Deserialize<Profile>(unescapedContent, JsonUtil.DefaultOptions);

                        if (profileContent is null) continue;
                    
                        if (!string.IsNullOrEmpty(profileContent.Name))
                        {
                            note.DisplayName = profileContent.Name;
                        }
                        if (!string.IsNullOrEmpty(profileContent.Picture))
                        {
                            note.ProfileImage = profileContent.Picture;
                        }
                        if (!string.IsNullOrEmpty(profileContent.Banner))
                        {
                            note.Banner = profileContent.Banner;
                        }
                        if (!string.IsNullOrEmpty(profileContent.About))
                        {
                            note.About = profileContent.About;
                        }

                        Notes.Add(note);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                _noteBatch.Clear();
                _isFetchingProfiles = false;
            });
        }

        private string ShortenPublicKey(string publicKey)
        {
            if (string.IsNullOrEmpty(publicKey)) return string.Empty;

            return publicKey.Length > 10 ? $"{publicKey[..6]}...{publicKey[^4..]}" : publicKey;
        }
    }
}
