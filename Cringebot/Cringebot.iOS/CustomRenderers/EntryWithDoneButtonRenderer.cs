using Cringebot.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryWithDoneButtonRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class EntryWithDoneButtonRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
                Control.ReturnKeyType = UIReturnKeyType.Done;
        }
    }
}