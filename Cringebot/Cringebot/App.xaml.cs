﻿using Xamarin.Forms;

namespace Cringebot
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var bootstrapper = new Bootstrapper();

            MainPage = bootstrapper.GetStartingPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
