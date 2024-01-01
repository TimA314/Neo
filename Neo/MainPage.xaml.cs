using Neo.Domain.Interfaces;
using Neo.Views;
using NostrNetTools.Nostr.Keys;
using System.Windows.Input;

namespace Neo
{
    public partial class MainPage : ContentPage
    {
        private readonly NostrKeyService _nostrKeyService;
        private readonly IUserKeyService _userKeyService;
        public ICommand LoginCommand { get; }
        public ICommand GenerateNewKeysCommand { get; }

        public MainPage(IUserKeyService userKeyService)
        {
            InitializeComponent();
            _nostrKeyService = new NostrKeyService();
            _userKeyService = userKeyService;

            LoginCommand = new Command(OnLoginClicked);
            GenerateNewKeysCommand = new Command(OnGenerateNewKeysClickedAsync);
            BindingContext = this;
        }

        private void OnGenerateNewKeysClickedAsync(object obj)
        {
            NostrKeySet nostrKeySet = _nostrKeyService.GenerateNewKeySet();
            _ = _userKeyService.SaveKeySet(nostrKeySet);
            Navigation.PushAsync(new EventsPage());
        }

        private void OnLoginClicked()
        {
            var privateKey = PrivateKeyEntry.Text.Trim();
            NostrKeySet nostrKeySet = _nostrKeyService.GenerateKeySetFromNSec(privateKey);
            _ = _userKeyService.SaveKeySet(nostrKeySet);

            Navigation.PushAsync(new EventsPage());
        }
    }
}
