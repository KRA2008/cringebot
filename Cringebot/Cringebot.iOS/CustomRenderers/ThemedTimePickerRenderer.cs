using System.Linq;
using Cringebot.iOS.CustomRenderers;
using Cringebot.ViewModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimePicker), typeof(ThemedTimePickerRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class ThemedTimePickerRenderer : TimePickerRenderer
    {
        private bool _initialized;

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                ApplyTheme(null);
                MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.THEME_EVENT, ApplyTheme);
                MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_EVENT, Unsubscribe);
                _initialized = true;
            }
        }

        private void Unsubscribe(SettingsViewModel vm)
        {
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, SettingsViewModel.THEME_EVENT);
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_EVENT);
        }

        private void ApplyTheme(SettingsViewModel vm)
        {
            var platformString = (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["styledFontShort"];
            var fontName = (string)platformString.Platforms.First(p => p.Platform.First() == "iOS").Value;
            Control.Font = UIFont.FromName(fontName, 16);
            var backgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["styledPageBackgroundColor"];
            Control.BackgroundColor = UIColor.FromRGB((int)(backgroundColor.R * 255), (int)(backgroundColor.G * 255), (int)(backgroundColor.B * 255));
            var textColor = (Color)Xamarin.Forms.Application.Current.Resources["styledTextColor"];
            Control.TextColor = UIColor.FromRGB((int)(textColor.R * 255), (int)(textColor.G * 255), (int)(textColor.B * 255));
        }
    }
}