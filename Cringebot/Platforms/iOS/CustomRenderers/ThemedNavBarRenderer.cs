using Cringebot.Services;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Cringebot.iOS.CustomRenderers
{
    public class ThemedNavBarRenderer : NavigationRenderer
    {
        private bool _initialized;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e); //TODO: check that they all have this
            if (!_initialized)
            {
                ApplyTheme(null);
                MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
                _initialized = true;
            }
        }

        private static void ApplyTheme(object obj)
        {
            //TODO
            //var font = Xamarin.Forms.Application.Current.Resources["styledFontShort"];
            //var att = new UITextAttributes
            //{
            //    Font = UIFont.FromName((string)font, 24)
            //};

            //UINavigationBar.Appearance.SetTitleTextAttributes(att);
        }
    }
}