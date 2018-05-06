using Android.Content;
using Android.Graphics;
using Android.Widget;
using Cringebot.Droid.CustomRenderers;
using Cringebot.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Color = Xamarin.Forms.Color;
using Toolbar = Android.Support.V7.Widget.Toolbar;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(ThemedNavBarRenderer))]
namespace Cringebot.Droid.CustomRenderers
{
    public class ThemedNavBarRenderer : NavigationPageRenderer
    {
        private Toolbar _toolbar;
        private TextView _textView;
        private bool _initialized;
        private readonly Context _context;

        public ThemedNavBarRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);
            if (!_initialized)
            {
                ApplyTheme(null);
                MessagingCenter.Subscribe<ThemeService>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
                _initialized = true;
            }
        }

        public override void OnViewAdded(Android.Views.View view)
        {
            base.OnViewAdded(view);
            if (view is Toolbar toolbar)
            {
                _toolbar = toolbar;
                _toolbar.ChildViewAdded += Toolbar_ChildViewAdded;
            }
        }

        private void Toolbar_ChildViewAdded(object sender, ChildViewAddedEventArgs e)
        {
            if (e.Child is TextView view)
            {
                _textView = view;
                _toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
            }
        }

        private void ApplyTheme(object obj)
        {
            if (_textView != null)
            {
                var fontString = (string) Application.Current.Resources["styledFontShort"];
                var font = Typeface.CreateFromAsset(_context.ApplicationContext.Assets, fontString);
                _textView.Typeface = font;
                _toolbar.SetSubtitleTextColor(Color.Red.ToAndroid());
            }
        }
    }
}