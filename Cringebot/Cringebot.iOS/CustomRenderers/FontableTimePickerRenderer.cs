using System.Linq;
using Cringebot.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimePicker), typeof(FontableTimePickerRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class FontableTimePickerRenderer : TimePickerRenderer
    {
        private bool _initialized;

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                var platformString = (OnPlatform<string>)Xamarin.Forms.Application.Current.Resources["styledFontShort"];
                var fontName = (string)platformString.Platforms.First(p => p.Platform.First() == "iOS").Value;
                Control.Font = UIFont.FromName(fontName, 16);
                var backgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["styledPageBackgroundColor"];
                Control.BackgroundColor = UIColor.FromRGB((int)(backgroundColor.R * 255), (int)(backgroundColor.G * 255), (int)(backgroundColor.B * 255));
                _initialized = true;
            }
        }
    }
}