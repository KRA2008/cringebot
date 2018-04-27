using System.Diagnostics;
using System.Threading.Tasks;
using Cringebot.iOS.CustomRenderers;
using Cringebot.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(ThemedNavBarRenderer))]
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
            //foreach (var familyName in UIFont.FamilyNames)
            //{
            //    Debug.WriteLine("### Family name: " + familyName);
            //    foreach (var fontName in UIFont.FontNamesForFamilyName(familyName))
            //    {
            //        Debug.WriteLine("### Font name: " + fontName);
            //    }
            //}

            var font = Xamarin.Forms.Application.Current.Resources["styledFontShort"];
            var textColor = Xamarin.Forms.Application.Current.Resources["styledNavBarTextColor"];
            var barColor = Xamarin.Forms.Application.Current.Resources["styledNavBarColor"];
            var att = new UITextAttributes
            {
                TextShadowColor = ((Color)barColor).ToUIColor(),
                TextColor = ((Color)textColor).ToUIColor(),
                Font = UIFont.FromName((string)font, 24),
                TextShadowOffset = new UIOffset()
            };

            UINavigationBar.Appearance.SetTitleTextAttributes(att);

            AppDelegate.Bootstrapper.ApplyTheme(null);
        }
    }
}