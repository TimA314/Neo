using Neo.ViewModels;

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
