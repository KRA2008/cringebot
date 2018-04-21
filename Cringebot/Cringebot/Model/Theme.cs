using Xamarin.Forms;

namespace Cringebot.Model
{
    public class Theme
    {
        public string Name { get; set; }
        public string AndroidFontLong { get; set; }
        public string AndroidFontShort { get; set; }
        public string iOSFont { get; set; }
        public Color TextColor { get; set; }
        public Color PlaceholderColor { get; set; }
        public Color NavBarColor { get; set; }
        public Color NavBarTextColor { get; set; }
        public Color PageBackgroundColor { get; set; }
        public Color ButtonBackgroundColor { get; set; }
        public Color ButtonTextColor { get; set; }
        public int ButtonCornerRadius { get; set; }
    }
}