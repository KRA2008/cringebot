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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg2NjkxNkAzMjM5MmUzMDJlMzAzYjMyMzkzYkQ5UTVMeit4NGxwQnBKR05YdW5zbmROWkl3RHN2NFpPZlA3TTBKaEkrVFE9");
            InitializeComponent();
            _themeService = themeService;
            _dataStore = persistentStorage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = FreshPageModelResolver.ResolvePageModel<MainViewModel>();
            _mainViewModel = (MainViewModel)mainPage.BindingContext;
            var window = new Window(new FreshNavigationContainer(mainPage));
            _themeService.ApplyTheme(_dataStore.LoadOrDefault(PersistentStorage.THEME_STORE_KEY, ""));
            return window;
        }

        public void Import(string import)
        {
            _mainViewModel.Import(import);
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
