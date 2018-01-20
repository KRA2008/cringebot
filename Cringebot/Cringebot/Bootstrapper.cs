using Cringebot.PageModel;
using Cringebot.Wrappers;
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
        }

        public Xamarin.Forms.Page GetStartingPage()
        {
            return new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainPageModel>());
        }
    }
}
