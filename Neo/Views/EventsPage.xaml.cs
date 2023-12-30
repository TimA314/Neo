using Neo.Domain.Models;

namespace Neo.Views
{
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
        {
            InitializeComponent();
            BindingContext = new EventsViewModel();
        }
    }
}
