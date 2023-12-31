using Neo.Models;

namespace Neo.Views;

public partial class ProfileDetailsPage : ContentPage
{
    public ProfileDetailsPage(Note note)
    {
        InitializeComponent();
        BindingContext = note;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}