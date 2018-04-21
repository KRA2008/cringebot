using System.Linq;
using Android.Content;
using Android.Graphics;
using Cringebot.Droid.CustomRenderers;
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
        }
    }
}