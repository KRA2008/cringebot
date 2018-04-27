using Cringebot.Services;
using Cringebot.Wrappers;
using Cringebot.ViewModel;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot
{
    public interface IBootstrapper
    {
        Xamarin.Forms.Page GetStartingPage();
    }

    public class Bootstrapper : IBootstrapper
    {
        // ReSharper disable MemberCanBeMadeStatic.Global - if static, no instantiation, if no instantiation, no registrations!
        private FreshNavigationContainer _navContainer;

        public App ResolveApp()
        {
            return FreshIOC.Container.Resolve<App>();
        }

        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppProperties, PropertiesWrapper>();
            FreshIOC.Container.Register(DependencyService.Get<INotificationManager>());
            FreshIOC.Container.Register<IDeviceWrapper, DeviceWrapper>().AsSingleton();
            FreshIOC.Container.Register(DependencyService.Get<IKeyboardHelper>());
            FreshIOC.Container.Register<IFileImportStore, FileImportStore>();
            FreshIOC.Container.Register<IThemeService, ThemeService>().AsSingleton();
            FreshIOC.Container.Register<IBootstrapper>(this);
        }
        
        public Xamarin.Forms.Page GetStartingPage()
        {
            _navContainer = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainViewModel>());
            if (Device.RuntimePlatform == Device.Android)
            {
                MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
            }
            ApplyTheme(null);
            return _navContainer;
        }

        public void ApplyTheme(object obj)
        {
            _navContainer.BarBackgroundColor = (Color) Application.Current.Resources["styledNavBarColor"];
            _navContainer.BarTextColor = (Color) Application.Current.Resources["styledNavBarTextColor"];
        }
        // ReSharper restore MemberCanBeMadeStatic.Global
    }
}
