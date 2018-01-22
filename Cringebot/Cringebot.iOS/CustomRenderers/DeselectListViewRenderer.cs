using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Cringebot.iOS.CustomRenderers;
using Cringebot.Page.CustomElements;

[assembly: ExportRenderer(typeof(DeselectListView), typeof(DeselectListViewRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class DeselectListViewRenderer : ListViewRenderer
    {
        private bool _isConfigured;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if(e.OldElement != null)
            {
                e.OldElement.ItemAppearing -= ItemAppearing;
                return;
            }

            if (e.NewElement == null || _isConfigured) return;

            _isConfigured = true;
            e.NewElement.ItemAppearing += ItemAppearing;
        }

        private static void ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var what = e.Item;
        }
    }
}