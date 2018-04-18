using Android.Content;
using Cringebot.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TableView), typeof(IdkRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class IdkRenderer : TableViewRenderer
    {
        private bool _initialized;

        public IdkRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);

            if (!_initialized)
            {
                _initialized = true;
            }
        }
    }
}