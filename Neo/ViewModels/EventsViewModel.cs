using Neo.Models;
using Neo.Services;
using Neo.Utilities;
using NostrNetTools.Nostr.Keys;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Neo.ViewModels
{
    public class EventsViewModel
    {
        private EventService _eventService;
        private bool _isFetchingProfiles;
        private List<Note> _noteBatch = [];



        public ObservableCollection<Note> Notes { get; set; }

        public EventsViewModel()
        {
            _eventService = new EventService("main-subscription", new { kinds = new[] { 1 } });
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

        private async void LoadNotes()
        {
            int BatchSize = 25;

            var kind1Filter = new
            {
                kinds = new[] { 1 },
                limit = 100,
            };

            EventService eventService = new("main-subscription", kind1Filter);
            _ = eventService.SubscribeToEvents(newEvent =>
            {
                _noteBatch.Add(new Note
                {
                    Id = newEvent.Id,
                    AuthorPublicKey = newEvent.PublicKey,
                    CreatedAt = GetTimeAgo(newEvent.CreatedAt),
                    Content = newEvent.Content,
                });

                if (_noteBatch.Count >= BatchSize || _noteBatch.Count >= 200)
                {
                    BatchSize += 25;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _ = FetchAndUpdateProfiles();
                    });
                }
            });
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
    }
}
