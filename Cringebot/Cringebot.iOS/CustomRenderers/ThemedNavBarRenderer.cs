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

        private void ApplyTheme(object obj)
        {
            //foreach (var familyName in UIFont.FamilyNames)
            //{
            //    Debug.WriteLine("### Family name: "+familyName);
            //    foreach (var font in UIFont.FontNamesForFamilyName(familyName))
            //    {
            //        Debug.WriteLine("### Font name: "+font);
            //    }
            //}
            
            var fontString = (string)Xamarin.Forms.Application.Current.Resources["styledFontShort"];
            var att = new UITextAttributes
            {
                Font = UIFont.FromName(fontString, 24)
            };
            Device.BeginInvokeOnMainThread(() =>
            {
                UINavigationBar.Appearance.SetTitleTextAttributes(att);
            });

            //var textColor = Xamarin.Forms.Application.Current.Resources["styledNavBarTextColor"];
            //var color = (Color)textColor;
            //UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
        }
    }
}