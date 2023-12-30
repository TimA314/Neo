using Neo.Models;
using Neo.Services;
using System.Collections.ObjectModel;

namespace Neo.ViewModels
{
    public class EventsViewModel
    {
        public ObservableCollection<EventDisplayModel> Events { get; set; }

        public EventsViewModel()
        {
            Events = new ObservableCollection<EventDisplayModel>();
            LoadEvents();
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

        private async void LoadEvents()
        {
            var filter = new
            {
                kinds = new[] { 1 },
                limit = 100,
            };

            EventService eventService = new EventService("my-subscription-id", filter);
            _ = eventService.ListenForEventsAsync(newEvent =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Events.Add(new EventDisplayModel
                    {
                        Id = newEvent.Id,
                        AuthorPublicKey = newEvent.PublicKey,
                        CreatedAt = GetTimeAgo(newEvent.CreatedAt),
                        Content = newEvent.Content,
                        // Add other properties as needed
                    });
                });
            });
        }
    }
}
