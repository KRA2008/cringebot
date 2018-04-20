using Cringebot.Wrappers;
using Cringebot.ViewModel;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppDataStore, StorageWrapper>();
            FreshIOC.Container.Register(DependencyService.Get<INotificationManager>());
            FreshIOC.Container.Register<IDeviceWrapper, DeviceWrapper>();
            FreshIOC.Container.Register(DependencyService.Get<IKeyboardHelper>());
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global - if static, no instantiation, if no instantiation, no registrations!
        public Xamarin.Forms.Page GetStartingPage()
        {
            return new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainViewModel>())
            {
                BarBackgroundColor = (Color)Application.Current.Resources["styledNavBarColor"],
                BarTextColor = (Color)Application.Current.Resources["styledNavBarTextColor"]
            };
        }
    }
}
