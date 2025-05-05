using Android.Content;
using Android.Graphics;
using Cringebot.Services;
using Cringebot.ViewModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Color = Microsoft.Maui.Graphics.Color;

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
                MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
                MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE, Unsubscribe);
                _initialized = true;
            }
        }

        private void Unsubscribe(object obj)
        {
            MessagingCenter.Unsubscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE);
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE);
        }

        private void ApplyTheme(object obj)
        {
            var font = Application.Current.Resources["styledFontShort"];
            if (font != null)
            {
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, (string)font);
            }
            var textColor = Application.Current.Resources["styledTextColor"];
            if (textColor != null)
            {
                var color = (Color) textColor;
                Control.SetTextColor(Android.Graphics.Color.Rgb((int)(color.Red * 255), (int)(color.Green * 255), (int)(color.Blue * 255)));
            }
        }
    }
}