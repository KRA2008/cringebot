using Cringebot.Services;
using Cringebot.Wrappers;
using Cringebot.ViewModel;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot
{
    public class Bootstrapper
    {
        // ReSharper disable MemberCanBeMadeStatic.Global - if static, no instantiation, if no instantiation, no registrations!
        private FreshNavigationContainer _navContainer;

        public App ResolveApp()
        {
            return FreshIOC.Container.Resolve<App>();
        }

        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppDataStore, StorageWrapper>().AsSingleton();
            FreshIOC.Container.Register(DependencyService.Get<INotificationManager>());
            FreshIOC.Container.Register<IDeviceWrapper, DeviceWrapper>().AsSingleton();
            FreshIOC.Container.Register(DependencyService.Get<IKeyboardHelper>());
            FreshIOC.Container.Register<IThemeService, ThemeService>().AsSingleton();
        }
        
        public Xamarin.Forms.Page GetStartingPage()
        {
            MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
            _navContainer = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainViewModel>());
            ApplyTheme(null);
            return _navContainer;
        }

        private void ApplyTheme(object obj)
        {
            _navContainer.BarBackgroundColor = (Color) Application.Current.Resources["styledNavBarColor"];
            _navContainer.BarTextColor = (Color) Application.Current.Resources["styledNavBarTextColor"];
        }
        // ReSharper restore MemberCanBeMadeStatic.Global
    }
}
