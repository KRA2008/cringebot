using Cringebot.PageModel;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot
{
    public partial class App : Application
    {
        private MainPageModel _mainPageModel;

        public App()
        {
            InitializeComponent();
            var bootstrapper = new Bootstrapper();

            var startingPage = bootstrapper.GetStartingPage();

            _mainPageModel = (MainPageModel)(((FreshBaseContentPage)(((FreshNavigationContainer)startingPage).CurrentPage)).BindingContext);

            MainPage = startingPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            _mainPageModel.Save();
        }

        protected override void OnResume()
        {
        }
    }
}
