using Android.Content;
using Android.Graphics;
using Cringebot.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TimePicker), typeof(FontableTimePickerRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class FontableTimePickerRenderer : TimePickerRenderer
    {
        private readonly Context _context;
        private bool _initialized;

        public FontableTimePickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, Application.Current.Resources["styledFontShort"].ToString());
                _initialized = true;
            }
        }
    }
}