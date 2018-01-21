using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Cringebot.Page.CustomElements;
using Cringebot.iOS.CustomRenderers;

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
            }

            if (e.NewElement != null && !_isConfigured)
            {
                _isConfigured = true;
                e.NewElement.ItemAppearing += ItemAppearing;
            }
        }

        private void ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var what = e.Item;
        }
    }
}