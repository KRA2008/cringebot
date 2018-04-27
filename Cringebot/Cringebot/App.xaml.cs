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
        private readonly IAppProperties _dataStore;
        private readonly MainViewModel _mainViewModel;

        public App(IBootstrapper bootstrapper, IThemeService themeService, IAppProperties dataStore)
        {
            InitializeComponent();
            _themeService = themeService;
            _dataStore = dataStore;

            var startingPage = bootstrapper.GetStartingPage();

            _mainViewModel = (MainViewModel)((FreshBaseContentPage)((FreshNavigationContainer)startingPage).CurrentPage).BindingContext;

            MainPage = startingPage;
        }

        public void Import(string import)
        {
            _mainViewModel.Import(import);
        }

        protected override void OnStart()
        {
            _themeService.ApplyTheme(_dataStore.LoadOrDefault(PropertiesWrapper.THEME_STORE_KEY, ""));
        }

        protected override void OnSleep()
        {
            _mainViewModel.Save();
            _dataStore.Save(PropertiesWrapper.THEME_STORE_KEY, _themeService.GetCurrentThemeName());
        }

        protected override void OnResume()
        {
        }
    }
}
