using System.Diagnostics;
using Android.App;
using Android.Content;
using Cringebot.MAUI;
using Activity = Android.App.Activity;

namespace Cringebot.Droid
{
    [Activity(Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(MainApplication.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}