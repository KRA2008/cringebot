using Cringebot.Services;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm.Maui;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Cringebot
{
    public partial class App : Application
    {
        private readonly IThemeService _themeService;
        private readonly IPersistentStorage _dataStore;
        private MainViewModel _mainViewModel;
        
        public App(IPersistentStorage persistentStorage, IThemeService themeService)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCe0x3THxbf1x1ZFRHalhXTnRdUj0eQnxTdEBjXH5ccndRQGJeUERyXElfag==");
            InitializeComponent();
            _themeService = themeService;
            _dataStore = persistentStorage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = FreshPageModelResolver.ResolvePageModel<MainViewModel>();
            _mainViewModel = (MainViewModel)mainPage.BindingContext;
            return new Window(new FreshNavigationContainer(mainPage));
        }

        public void Import(string import)
        {
            _mainViewModel.Import(import);
        }

        protected override void OnStart()
        {
            _themeService.ApplyTheme(_dataStore.LoadOrDefault(PersistentStorage.THEME_STORE_KEY, ""));
        }

        protected override void OnSleep()
        {
            _mainViewModel.Save();
            _dataStore.Save(PersistentStorage.THEME_STORE_KEY, _themeService.GetCurrentThemeName());
        }

        protected override void OnResume()
        {
        }
    }
}
