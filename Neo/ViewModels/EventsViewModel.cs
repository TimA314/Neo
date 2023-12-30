using Neo.Infrastructure.Services;
using Neo.Models;
using System.Collections.ObjectModel;

namespace Neo.Domain.Models
{
    public class EventsViewModel
    {
        public ObservableCollection<EventDisplayModel> Events { get; set; }

        public EventsViewModel()
        {
            Events = new ObservableCollection<EventDisplayModel>();
            LoadEvents();
        }

        private async void LoadEvents()
        {
            var filter = new
            {
                kinds = new[] { 1 },
                limit = 10,
            };

            EventService eventService = new EventService("my-subscription-id", filter);
            var events = await eventService.GetEventsAsync();
            foreach (var newEvent in events)
            {
                Events.Add(new EventDisplayModel
                {
                    Id = newEvent.Id,
                    AuthorPublicKey = newEvent.PublicKey,
                    CreatedAt = newEvent.CreatedAt.HasValue ? newEvent.CreatedAt.Value.UtcDateTime : DateTime.MinValue,
                    Content = newEvent.Content,
                    Images = new List<string>(),
                    Tags = newEvent.Tags.Select(t => t.TagIdentifier).ToList()

                });
            }
        }
    }
}
