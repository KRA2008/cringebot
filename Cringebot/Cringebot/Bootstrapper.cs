using Cringebot.PageModel;
using Cringebot.Wrappers;
using FreshMvvm;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppDataStore, StorageWrapper>();
        }

        public Xamarin.Forms.Page GetStartingPage()
        {
            return new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainPageModel>());
        }
    }
}
