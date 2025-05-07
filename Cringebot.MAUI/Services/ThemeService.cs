using Cringebot.Model;
using Cringebot.Wrappers;

namespace Cringebot.Services
{
    public interface IThemeService
    {
        void ApplyTheme(string name);
        IEnumerable<string> GetThemes();
        string GetCurrentThemeName();
    }

    public class ThemeService : IThemeService
    {
        public const string TOOLS_SHOULD_BE_BLACK_CHANGED = "toolsBlackChanged";
        public const string THEME_SET_MESSAGE = "themeSet";
        private const double REQUIRED_LUMINANCE_DIFFERENCE = 0.2;
        private const string BLACK_ADD_NAME = "add";
        private const string WHITE_ADD_NAME = "addwhite";
        private const string BLACK_DELETE_NAME = "clear";
        private const string WHITE_DELETE_NAME = "clearwhite";
        private const string WHITE_LIST_NAME = "list";
        private const string BLACK_LIST_NAME = "listblack";

        private readonly IDeviceWrapper _deviceWrapper;
        private readonly Theme[] _themes;
        private string _currentThemeName;
        private readonly Random _random;

        public ThemeService(IDeviceWrapper deviceWrapper)
        {
            _random = new Random();
            _deviceWrapper = deviceWrapper;
            _themes = new[]
            {
                new Theme
                {
                    Name = "Cringe",
                    FontName = "ComicSansMS",
                    FontFileName = "Comic Sans MS.ttf",
                    ButtonCornerRadius = 12,
                    SmallestTextSize = 14,
                    MediumestTextSize = 16,
                    LargestTextSize = 18
                },
                new Theme
                {
                    Name = "Goth",
                    FontName = "PlainBlack-Normal",
                    FontFileName = "Plain Black.ttf",
                    TextColor = Colors.White,
                    HighlightTextColor = Colors.White,
                    ButtonTextColor = Colors.Red,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Colors.Black,
                    ButtonBackgroundColor = Colors.White,
                    PlaceholderColor = Colors.Red,
                    NavBarColor = Colors.DarkRed,
                    NavBarTextColor = Colors.Black,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 18,
                    MediumestTextSize = 20,
                    LargestTextSize = 22
                },
                new Theme
                {
                    Name = "Mac & Cheese",
                    FontName = "SBCMacaroni",
                    FontFileName = "SBC Macaroni Regular.ttf",
                    TextColor = Colors.OrangeRed,
                    HighlightTextColor = Colors.OrangeRed,
                    ButtonTextColor = Colors.Yellow,
                    PageBackgroundColor = Colors.Yellow,
                    PageBackgroundImageName = "",
                    ButtonBackgroundColor = Colors.Orange,
                    NavBarColor = Colors.Orange,
                    NavBarTextColor = Colors.Yellow,
                    PlaceholderColor = Colors.Orange,
                    ButtonCornerRadius = 25,
                    SmallestTextSize = 16,
                    MediumestTextSize = 18,
                    LargestTextSize = 20
                },
                new Theme
                {
                    Name = "Emo",
                    FontName = "rise up",
                    FontFileName = "riseup.ttf",
                    TextColor = Colors.DarkRed,
                    HighlightTextColor = Colors.DarkRed,
                    ButtonTextColor = Colors.DarkRed,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Colors.Black,
                    ButtonBackgroundColor = Colors.White,
                    PlaceholderColor = Colors.Red,
                    NavBarColor = Colors.DarkRed,
                    NavBarTextColor = Colors.Black,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 14,
                    MediumestTextSize = 16,
                    LargestTextSize = 18
                },
                new Theme
                {
                    Name = "Black and White",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Colors.Black,
                    HighlightTextColor = Colors.Black,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Colors.White,
                    ButtonTextColor = Colors.White,
                    ButtonBackgroundColor = Colors.DimGray,
                    PlaceholderColor = Colors.Gray,
                    NavBarColor = Colors.Black,
                    NavBarTextColor = Colors.White,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 14,
                    MediumestTextSize = 16,
                    LargestTextSize = 18
                },
                new Theme
                {
                    Name = "America",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Colors.Blue,
                    HighlightTextColor = Colors.Blue,
                    ButtonTextColor = Colors.White,
                    PageBackgroundImageName = "stripes",
                    PageBackgroundColor = Colors.Transparent,
                    ButtonBackgroundColor = Colors.Blue,
                    PlaceholderColor = Colors.CornflowerBlue,
                    NavBarColor = Colors.Blue,
                    NavBarTextColor = Colors.White,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 16,
                    MediumestTextSize = 18,
                    LargestTextSize = 20
                },
                new Theme
                {
                    Name = "Burger",
                    FontName = "Condiment-Regular",
                    FontFileName = "Condiment.ttf",
                    TextColor = Colors.Yellow,
                    HighlightTextColor = Colors.Yellow,
                    ButtonTextColor = Colors.GreenYellow,
                    PageBackgroundImageName = "", //TODO: make this a picture of beef
                    PageBackgroundColor = Colors.SaddleBrown,
                    ButtonBackgroundColor = Colors.Green,
                    PlaceholderColor = Colors.Green,
                    NavBarColor = Colors.Red,
                    NavBarTextColor = Colors.Yellow,
                    ButtonCornerRadius = 25,
                    SmallestTextSize = 18,
                    MediumestTextSize = 20,
                    LargestTextSize = 22
                }
            };
        }

        public void ApplyTheme(string name)
        {
            var targetTheme = _themes.FirstOrDefault(t => t.Name == name) ?? _themes.First();

            _currentThemeName = targetTheme.Name;
            var isiOS = _deviceWrapper.RuntimePlatform() == Device.iOS;
            var isCringe = Array.IndexOf(_themes, targetTheme) == 0;

            var textColor = isCringe ? GetRandomColor() : targetTheme.TextColor;
            var backgroundColor = isCringe ? GetRandomColor() : targetTheme.PageBackgroundColor;
            var placeholderColor = isCringe ? GetRandomColor() : targetTheme.PlaceholderColor;
            var highlightColor = isCringe ? GetRandomColor() : targetTheme.TextColor;
            if (isCringe)
            {
                LoopUntilLuminanceDifferenceSatisfied(ref backgroundColor, ref textColor);
                LoopUntilLuminanceDifferenceSatisfied(ref backgroundColor, ref placeholderColor);
                LoopUntilLuminanceDifferenceSatisfied(ref backgroundColor, ref highlightColor);
            }

            Application.Current.Resources["styledPageBackgroundColor"] = backgroundColor;
            Application.Current.Resources["styledTextColor"] = textColor;
            //TODO
            //Application.Current.Resources["styledChartColors"] = new ChartColorCollection
            //{
            //    textColor
            //};
            Application.Current.Resources["styledPlaceholderColor"] = placeholderColor;
            Application.Current.Resources["styledHighlightTextColor"] = highlightColor;

            var buttonColor = isCringe ? GetRandomColor() : targetTheme.ButtonBackgroundColor;
            var buttonTextColor = isCringe ? GetRandomColor() : targetTheme.ButtonTextColor;
            if (isCringe)
            {
                LoopUntilLuminanceDifferenceSatisfied(ref buttonColor, ref buttonTextColor);
            }

            Application.Current.Resources["styledButtonBackgroundColor"] = buttonColor;
            Application.Current.Resources["styledButtonTextColor"] = buttonTextColor;

            var navBarColor = isCringe ? GetRandomColor() : targetTheme.NavBarColor;
            var navBarTextColor = isCringe ? GetRandomColor() : targetTheme.NavBarTextColor;
            if (isCringe)
            {
                LoopUntilLuminanceDifferenceSatisfied(ref navBarColor, ref navBarTextColor);
            }

            Application.Current.Resources["styledNavBarColor"] = navBarColor;
            Application.Current.Resources["styledNavBarTextColor"] = navBarTextColor;

            Application.Current.Resources["styledButtonCornerRadius"] = targetTheme.ButtonCornerRadius;
            
            Application.Current.Resources["styledFontShort"] = isiOS ? targetTheme.FontName : targetTheme.FontFileName;
            Application.Current.Resources["styledFontLong"] = isiOS ? targetTheme.FontName : targetTheme.FontFileName + "#" + targetTheme.FontName;

            Application.Current.Resources["smallestTextSize"] = targetTheme.SmallestTextSize;
            Application.Current.Resources["mediumestTextSize"] = targetTheme.MediumestTextSize;
            Application.Current.Resources["largestTextSize"] = targetTheme.LargestTextSize;

            Application.Current.Resources["styledPageBackgroundImageName"] = targetTheme.PageBackgroundImageName;

            if (!isiOS && Math.Abs(GetLuminance(backgroundColor) - GetLuminance(Colors.Black)) < REQUIRED_LUMINANCE_DIFFERENCE)
            {
                Application.Current.Resources["addImageName"] = WHITE_ADD_NAME;
                Application.Current.Resources["deleteImageName"] = WHITE_DELETE_NAME;
            }
            else
            {
                Application.Current.Resources["addImageName"] = BLACK_ADD_NAME;
                Application.Current.Resources["deleteImageName"] = BLACK_DELETE_NAME;
            }

            if (isiOS || Math.Abs(GetLuminance(navBarColor) - GetLuminance(Colors.Black)) < REQUIRED_LUMINANCE_DIFFERENCE)
            {
                MessagingCenter.Send(this, TOOLS_SHOULD_BE_BLACK_CHANGED, false);
                Application.Current.Resources["statListIconName"] = WHITE_LIST_NAME;
            }
            else
            {
                MessagingCenter.Send(this, TOOLS_SHOULD_BE_BLACK_CHANGED, true);
                Application.Current.Resources["statListIconName"] = BLACK_LIST_NAME;
            }

            MessagingCenter.Send(this, THEME_SET_MESSAGE);
        }

        private static double GetLuminance(Color color)
        {
            return Math.Sqrt(0.299 * Math.Pow(color.Red, 2) + 0.587 * Math.Pow(color.Green, 2) + 0.114 * Math.Pow(color.Blue, 2));
        }

        private void LoopUntilLuminanceDifferenceSatisfied(ref Color colorA, ref Color colorB)
        {
            var luminanceA = GetLuminance(colorA);
            while (Math.Abs(GetLuminance(colorB) - luminanceA) < REQUIRED_LUMINANCE_DIFFERENCE)
            {
                colorB = GetRandomColor();
            }
        }

        private Color GetRandomColor()
        {
            return Color.FromRgb(
                (byte)_random.Next(0, 256), 
                (byte)_random.Next(0, 256), 
                (byte)_random.Next(0, 256));
        }

        public IEnumerable<string> GetThemes()
        {
            return _themes.Select(t => t.Name);
        }

        public string GetCurrentThemeName()
        {
            return _currentThemeName;
        }
    }
}