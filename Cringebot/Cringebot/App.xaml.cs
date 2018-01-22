using Cringebot.ViewModel;
using FreshMvvm;

namespace Cringebot
{
    public partial class App
    {
        private readonly MainViewModel _mainViewModel;

        public App()
        {
            InitializeComponent();
            var bootstrapper = new Bootstrapper();

            var startingPage = bootstrapper.GetStartingPage();

            _mainViewModel = (MainViewModel)((FreshBaseContentPage)((FreshNavigationContainer)startingPage).CurrentPage).BindingContext;

            MainPage = startingPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            _mainViewModel.Save();
        }

        protected override void OnResume()
        {
        }
    }
}
