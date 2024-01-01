namespace Neo
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceProvider = serviceProvider;
            MainPage = new NavigationPage(serviceProvider.GetService<MainPage>());
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Width = 800;
            return window;
        }
    }
}
