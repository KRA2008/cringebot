using Cringebot.Services;
using Cringebot.ViewModel;
using Cringebot.Wrappers;
using FreshMvvm.Maui;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Cringebot
{
    public partial class App
    {
        private static IThemeService _themeService;
        private static IPersistentStorage _dataStore;
        public static MainViewModel? MainViewModel;
        
        public App(IPersistentStorage persistentStorage, IThemeService themeService)
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9feHZVRWJYU0VwXktWYEg=");
            _themeService = themeService;
            _dataStore = persistentStorage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = FreshPageModelResolver.ResolvePageModel<MainViewModel>();
            MainViewModel = (MainViewModel)mainPage.BindingContext;
            var window = new Window(new FreshNavigationContainer(mainPage));
            _themeService.ApplyTheme(_dataStore.LoadOrDefault(PersistentStorage.THEME_STORE_KEY, ""));
            return window;
        }

        public void Import(string import)
        {
            MainViewModel.Import(import);
        }
    }
}
