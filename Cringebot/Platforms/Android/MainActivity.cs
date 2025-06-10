using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Cringebot.Droid
{
    [Activity(Label = "Cringebot", Icon = "@drawable/icon", Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
        }
    }
}

