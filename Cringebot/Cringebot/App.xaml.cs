using Cringebot.Services;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Cringebot
{
    public partial class App
    {
        private readonly IThemeService _themeService;
        private readonly IAppDataStore _dataStore;
        private readonly MainViewModel _mainViewModel;

        public App(IBootstrapper bootstrapper, IThemeService themeService, IAppDataStore dataStore)
        {
            InitializeComponent();
            _themeService = themeService;
            _dataStore = dataStore;

            var startingPage = bootstrapper.GetStartingPage();

            _mainViewModel = (MainViewModel)((FreshBaseContentPage)((FreshNavigationContainer)startingPage).CurrentPage).BindingContext;

            MainPage = startingPage;
        }

        protected override void OnStart()
        {
            _themeService.ApplyTheme(_dataStore.LoadOrDefault(StorageWrapper.THEME_STORE_KEY, ""));
        }

        protected override void OnSleep()
        {
            _mainViewModel.Save();
            _dataStore.Save(StorageWrapper.THEME_STORE_KEY, _themeService.GetCurrentThemeName());
        }

        protected override void OnResume()
        {
        }
    }
}
