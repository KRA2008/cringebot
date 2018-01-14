using Cringebot.PageModel;
using Cringebot.Utilities;
using Cringebot.Wrappers;
using FreshMvvm;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            FreshIOC.Container.Register<IAppDataStore, StorageWrapper>();
            FreshIOC.Container.Register<IInStorageMemoryUpdater, InStorageMemoryUpdater>();
        }

        public Xamarin.Forms.Page GetStartingPage()
        {
            return new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<MainPageModel>());
        }
    }
}
