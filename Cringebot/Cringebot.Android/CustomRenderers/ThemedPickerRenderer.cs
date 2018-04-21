using Android.Content;
using Android.Graphics;
using Cringebot.Droid.CustomRenderers;
using Cringebot.Services;
using Cringebot.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PickerRenderer = Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer;

[assembly: ExportRenderer(typeof(Picker), typeof(ThemedPickerRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class ThemedPickerRenderer : PickerRenderer
    {
        private readonly Context _context;
        private bool _initialized;

        public ThemedPickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                ApplyTheme(null);
                MessagingCenter.Subscribe<SettingsViewModel>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
                MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE, Unsubscribe);
                _initialized = true;
            }
        }

        private void Unsubscribe(SettingsViewModel vm)
        {
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, ThemeService.THEME_SET_MESSAGE);
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE);
        }

        private void ApplyTheme(SettingsViewModel vm)
        {
            var font = Application.Current.Resources["styledFontShort"];
            if (font != null)
            {
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, (string)font);
            }
        }
    }
}