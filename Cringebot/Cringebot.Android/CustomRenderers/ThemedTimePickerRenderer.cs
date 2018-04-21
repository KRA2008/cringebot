using System.Linq;
using Android.Content;
using Android.Graphics;
using Cringebot.Droid.CustomRenderers;
using Cringebot.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(TimePicker), typeof(ThemedTimePickerRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class ThemedTimePickerRenderer : TimePickerRenderer
    {
        private readonly Context _context;
        private bool _initialized;

        public ThemedTimePickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

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
            var platformString = (OnPlatform<string>)Application.Current.Resources["styledFontShort"];
            var fontName = (string)platformString.Platforms.First(p => p.Platform.First() == "Android").Value;
            Control.Typeface = Typeface.CreateFromAsset(_context.Assets, fontName);
            var textColor = (Color)Application.Current.Resources["styledTextColor"];
            Control.SetTextColor(Android.Graphics.Color.Rgb((int)(textColor.R * 255), (int)(textColor.G * 255), (int)(textColor.B * 255)));
        }
    }
}