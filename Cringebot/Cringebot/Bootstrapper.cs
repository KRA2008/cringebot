using Cringebot.ViewModel;
using Cringebot.Wrappers;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            FreshMvvm.FreshIOC.Container.Register<IAppDataStore, StorageWrapper>();
        }

        public Xamarin.Forms.Page GetStartingPage()
        {
            return FreshMvvm.FreshPageModelResolver.ResolvePageModel<MainViewModel>();
        }
    }
}
