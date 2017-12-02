using Cringebot.ViewModel;

namespace Cringebot
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {

        }

        public Xamarin.Forms.Page GetStartingPage()
        {
            return FreshMvvm.FreshPageModelResolver.ResolvePageModel<MainViewModel>();
        }
    }
}
