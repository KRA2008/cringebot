using System.Linq;
using Android.Content;
using Android.Graphics;
using Cringebot.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PickerRenderer = Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer;

[assembly: ExportRenderer(typeof(Picker), typeof(FontablePickerRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class FontablePickerRenderer : PickerRenderer
    {
        private readonly Context _context;
        private bool _initialized;

        public FontablePickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                var platformString = (OnPlatform<string>)Application.Current.Resources["styledFontShort"];
                var fontName = (string)platformString.Platforms.First(p => p.Platform.First() == "Android").Value;
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, fontName);
                _initialized = true;
            }
        }
    }
}