using CoreGraphics;
using Cringebot.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryWithDoneButtonRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class EntryWithDoneButtonRenderer : EntryRenderer
    {
        private bool _initialized;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (!_initialized && Control != null && e.NewElement != null)
            {
                var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f))
                {
                    Items = new[]
                    {
                        new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                        new UIBarButtonItem(UIBarButtonSystemItem.Cancel, delegate { Control.ResignFirstResponder(); })
                    }
                };

                Control.InputAccessoryView = toolbar;
                Control.ReturnKeyType = UIReturnKeyType.Done;
                _initialized = true;
            }
        }
    }
}