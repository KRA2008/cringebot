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
        
        public App(IThemeService themeService, IPersistentStorage dataStore)
        {
            InitializeComponent();
            _themeService = themeService;
            _dataStore = dataStore;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainWindow = FreshPageModelResolver.ResolvePageModel<MainViewModel>();
            _mainViewModel = (MainViewModel)mainWindow.BindingContext;
            return new Window(mainWindow);
        }

        public void Import(string import)
        {
            _mainViewModel.Import(import);
        }

        //protected override void OnStart()
        //{
        //    _themeService.ApplyTheme(_dataStore.LoadOrDefault(PersistentStorage.THEME_STORE_KEY, ""));
        //}

        //protected override void OnSleep()
        //{
        //    _mainViewModel.Save();
        //    _dataStore.Save(PersistentStorage.THEME_STORE_KEY, _themeService.GetCurrentThemeName());
        //}

        //protected override void OnResume()
        //{
        //}
    }
}
