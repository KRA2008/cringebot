﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Cringebot.Droid
{
    [Activity(Label = "Cringebot", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            var bootstrapper = new Bootstrapper();
            LoadApplication(bootstrapper.ResolveApp());
        }
    }
}

