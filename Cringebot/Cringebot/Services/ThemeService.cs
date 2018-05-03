using System;
using System.Collections.Generic;
using System.Linq;
using Cringebot.Model;
using Cringebot.Wrappers;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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
        public const string THEME_SET_MESSAGE = "themeSet";
        private const double REQUIRED_LUMINANCE_DIFFERENCE = 0.2;
        private const string BLACK_ADD_NAME = "add";
        private const string WHITE_ADD_NAME = "addwhite";
        private const string BLACK_DELETE_NAME = "clear";
        private const string WHITE_DELETE_NAME = "clearwhite";

        private readonly IDeviceWrapper _deviceWrapper;
        private readonly IEnumerable<Theme> _themes;
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
                    LargestTextSize = 18,
                    AddImageName = BLACK_ADD_NAME,
                    DeleteImageName = BLACK_DELETE_NAME
                },
                new Theme
                {
                    Name = "Goth",
                    FontName = "PlainBlack-Normal",
                    FontFileName = "Plain Black.ttf",
                    TextColor = Color.White,
                    HighlightTextColor = Color.White,
                    ButtonTextColor = Color.Red,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Color.Black,
                    ButtonBackgroundColor = Color.White,
                    PlaceholderColor = Color.Red,
                    NavBarColor = Color.DarkRed,
                    NavBarTextColor = Color.Black,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 18,
                    MediumestTextSize = 20,
                    LargestTextSize = 22,
                    AddImageName = WHITE_ADD_NAME,
                    DeleteImageName = WHITE_DELETE_NAME
                },
                new Theme
                {
                    Name = "Mac & Cheese",
                    FontName = "SBCMacaroni",
                    FontFileName = "SBC Macaroni Regular.ttf",
                    TextColor = Color.OrangeRed,
                    HighlightTextColor = Color.OrangeRed,
                    ButtonTextColor = Color.Yellow,
                    PageBackgroundColor = Color.Yellow,
                    PageBackgroundImageName = "",
                    ButtonBackgroundColor = Color.Orange,
                    NavBarColor = Color.Orange,
                    NavBarTextColor = Color.Yellow,
                    PlaceholderColor = Color.Orange,
                    ButtonCornerRadius = 25,
                    SmallestTextSize = 16,
                    MediumestTextSize = 18,
                    LargestTextSize = 20,
                    AddImageName = BLACK_ADD_NAME,
                    DeleteImageName = BLACK_DELETE_NAME
                },
                new Theme
                {
                    Name = "Emo",
                    FontName = "rise up",
                    FontFileName = "riseup.ttf",
                    TextColor = Color.DarkRed,
                    HighlightTextColor = Color.DarkRed,
                    ButtonTextColor = Color.DarkRed,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Color.Black,
                    ButtonBackgroundColor = Color.White,
                    PlaceholderColor = Color.Red,
                    NavBarColor = Color.DarkRed,
                    NavBarTextColor = Color.Black,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 14,
                    MediumestTextSize = 16,
                    LargestTextSize = 18,
                    AddImageName = WHITE_ADD_NAME,
                    DeleteImageName = WHITE_DELETE_NAME
                },
                new Theme
                {
                    Name = "Black and White",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Color.Black,
                    HighlightTextColor = Color.Black,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Color.White,
                    ButtonTextColor = Color.White,
                    ButtonBackgroundColor = Color.DimGray,
                    PlaceholderColor = Color.Gray,
                    NavBarColor = Color.Black,
                    NavBarTextColor = Color.White,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 14,
                    MediumestTextSize = 16,
                    LargestTextSize = 18,
                    AddImageName = BLACK_ADD_NAME,
                    DeleteImageName = BLACK_DELETE_NAME
                },
                new Theme
                {
                    Name = "America",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Color.Blue,
                    HighlightTextColor = Color.Blue,
                    ButtonTextColor = Color.White,
                    PageBackgroundImageName = "stripes",
                    PageBackgroundColor = Color.Transparent,
                    ButtonBackgroundColor = Color.Blue,
                    PlaceholderColor = Color.CornflowerBlue,
                    NavBarColor = Color.Blue,
                    NavBarTextColor = Color.White,
                    ButtonCornerRadius = 0,
                    SmallestTextSize = 16,
                    MediumestTextSize = 18,
                    LargestTextSize = 20,
                    AddImageName = BLACK_ADD_NAME,
                    DeleteImageName = BLACK_DELETE_NAME
                },
                new Theme
                {
                    Name = "Burger",
                    FontName = "Condiment-Regular",
                    FontFileName = "Condiment.ttf",
                    TextColor = Color.Yellow,
                    HighlightTextColor = Color.Yellow,
                    ButtonTextColor = Color.GreenYellow,
                    PageBackgroundImageName = "", //TODO: make this a picture of beef
                    PageBackgroundColor = Color.SaddleBrown,
                    ButtonBackgroundColor = Color.Green,
                    PlaceholderColor = Color.Green,
                    NavBarColor = Color.Red,
                    NavBarTextColor = Color.Yellow,
                    ButtonCornerRadius = 25,
                    SmallestTextSize = 18,
                    MediumestTextSize = 20,
                    LargestTextSize = 22,
                    AddImageName = BLACK_ADD_NAME,
                    DeleteImageName = BLACK_DELETE_NAME
                }
            };
        }

        public void ApplyTheme(string name)
        {
            var targetTheme = _themes.FirstOrDefault(t => t.Name == name) ?? _themes.First();

            _currentThemeName = targetTheme.Name;
            var isiOS = _deviceWrapper.RuntimePlatform() == Device.iOS;
            var isCringe = _themes.IndexOf(targetTheme) == 0;

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
            Application.Current.Resources["styledChartColors"] = new ChartColorCollection
            {
                textColor
            };
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

            Application.Current.Resources["addImageName"] = targetTheme.AddImageName;
            Application.Current.Resources["deleteImageName"] = targetTheme.DeleteImageName;

            MessagingCenter.Send(this, THEME_SET_MESSAGE);
        }

        private void LoopUntilLuminanceDifferenceSatisfied(ref Color colorA, ref Color colorB)
        {
            var luminanceA = GetLuminance(colorA);
            while (Math.Abs(GetLuminance(colorB) - luminanceA) < REQUIRED_LUMINANCE_DIFFERENCE)
            {
                colorB = GetRandomColor();
            }
        }

        private static double GetLuminance(Color color)
        {
            return Math.Sqrt(0.299 * Math.Pow(color.R, 2) + 0.587 * Math.Pow(color.G, 2) + 0.114 * Math.Pow(color.B, 2));
        }

        private Color GetRandomColor()
        {
            return Color.FromRgb(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));
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