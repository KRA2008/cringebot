using Xamarin.Forms;

namespace Cringebot.Model
{
    public class Theme
    {
        public string Name { get; set; }
        public string FontName { get; set; }
        public string FontFileName { get; set; }
        public Color TextColor { get; set; }
        public Color PlaceholderColor { get; set; }
        public Color NavBarColor { get; set; }
        public Color NavBarTextColor { get; set; }
        public Color PageBackgroundColor { get; set; }
        public string PageBackgroundImageName { get; set; }
        public Color ButtonBackgroundColor { get; set; }
        public Color ButtonTextColor { get; set; }
        public int ButtonCornerRadius { get; set; }
        public double SmallestTextSize { get; set; }
        public double MediumestTextSize { get; set; }
        public double LargestTextSize { get; set; }
    }
}