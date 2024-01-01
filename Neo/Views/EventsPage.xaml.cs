using Neo.Models;
using Neo.ViewModels;

namespace Neo.Views
{
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
        {
            InitializeComponent();
            var app = Application.Current as App;
            BindingContext = app?.ServiceProvider.GetService<EventsViewModel>();
        }

        private async void OnDisplayNameTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Note note)
            {
                var profilePage = new ProfileDetailsPage(note);
                await Navigation.PushModalAsync(profilePage);
            }
        }
    }
}
