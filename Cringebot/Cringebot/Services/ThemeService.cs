using System;
using System.Collections.Generic;
using System.Linq;
using Cringebot.Model;
using Cringebot.Wrappers;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;

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

        private readonly IDeviceWrapper _deviceWrapper;
        private readonly IEnumerable<Theme> _themes;
        private string _currentThemeName;
        private readonly Random _random;

        public ThemeService(IDeviceWrapper deviceWrapper)
        {
            //TODO: change icons white and black?
            _random = new Random();
            _deviceWrapper = deviceWrapper;
            _themes = new[]
            {
                new Theme
                {
                    Name = "Cringe",
                    FontName = "ComicSansMS",
                    FontFileName = "Comic Sans MS.ttf",
                    TextColor = Color.Yellow,
                    PlaceholderColor = Color.DimGray,
                    NavBarColor = Color.Red,
                    NavBarTextColor = Color.Green,
                    PageBackgroundImageName = "",
                    PageBackgroundColor = Color.Turquoise,
                    ButtonBackgroundColor = Color.Yellow,
                    ButtonTextColor = Color.DeepPink,
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
                    TextColor = Color.White,
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
                    LargestTextSize = 22
                },
                new Theme
                {
                    Name = "Mac & Cheese",
                    FontName = "SBCMacaroni",
                    FontFileName = "SBC Macaroni Regular.ttf",
                    TextColor = Color.OrangeRed,
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
                    LargestTextSize = 20
                },
                new Theme
                {
                    Name = "Emo",
                    FontName = "rise up",
                    FontFileName = "riseup.ttf",
                    TextColor = Color.DarkRed,
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
                    LargestTextSize = 18
                },
                new Theme
                {
                    Name = "Black and White",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Color.Black,
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
                    LargestTextSize = 18
                },
                new Theme
                {
                    Name = "America",
                    FontName = "TimesNewRomanPSMT",
                    FontFileName = "times.ttf",
                    TextColor = Color.Blue,
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
                    LargestTextSize = 20
                },
                new Theme
                {
                    Name = "Burger",
                    FontName = "Condiment-Regular",
                    FontFileName = "Condiment.ttf",
                    TextColor = Color.Yellow,
                    ButtonTextColor = Color.White,
                    PageBackgroundImageName = "", //TODO: make this a picture of beef
                    PageBackgroundColor = Color.SaddleBrown,
                    ButtonBackgroundColor = Color.Green,
                    PlaceholderColor = Color.Green,
                    NavBarColor = Color.Red,
                    NavBarTextColor = Color.Yellow,
                    ButtonCornerRadius = 25,
                    SmallestTextSize = 18,
                    MediumestTextSize = 20,
                    LargestTextSize = 22
                },
                new Theme
                {
                    Name = "Random",
                    ButtonCornerRadius = 0,
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
            var isRandom = name == "Random";

            Application.Current.Resources["styledTextColor"] = isRandom ? GetRandomColor() : targetTheme.TextColor;
            Application.Current.Resources["styledChartColors"] = new ChartColorCollection
            {
                isRandom ? GetRandomColor() : targetTheme.TextColor
            };
            Application.Current.Resources["styledPageBackgroundImageName"] = targetTheme.PageBackgroundImageName;
            Application.Current.Resources["styledPageBackgroundColor"] = isRandom ? GetRandomColor() : targetTheme.PageBackgroundColor;
            Application.Current.Resources["styledButtonBackgroundColor"] = isRandom ? GetRandomColor() : targetTheme.ButtonBackgroundColor;
            Application.Current.Resources["styledButtonTextColor"] = isRandom ? GetRandomColor() : targetTheme.ButtonTextColor;
            Application.Current.Resources["styledPlaceholderColor"] = isRandom ? GetRandomColor() : targetTheme.PlaceholderColor;
            Application.Current.Resources["styledNavBarColor"] = isRandom ? GetRandomColor() : targetTheme.NavBarColor;
            Application.Current.Resources["styledNavBarTextColor"] = isRandom ? GetRandomColor() : targetTheme.NavBarTextColor;

            Application.Current.Resources["styledButtonCornerRadius"] = targetTheme.ButtonCornerRadius;

            var fontTargetTheme = isRandom ? _themes.ElementAt(_random.Next(0, _themes.Count()-1)) : targetTheme;
            Application.Current.Resources["styledFontShort"] = isiOS ? fontTargetTheme.FontName : fontTargetTheme.FontFileName;
            Application.Current.Resources["styledFontLong"] = isiOS ? fontTargetTheme.FontName : fontTargetTheme.FontFileName + "#" + fontTargetTheme.FontName;

            Application.Current.Resources["smallestTextSize"] = targetTheme.SmallestTextSize;
            Application.Current.Resources["mediumestTextSize"] = targetTheme.MediumestTextSize;
            Application.Current.Resources["largestTextSize"] = targetTheme.LargestTextSize;

            MessagingCenter.Send(this, THEME_SET_MESSAGE);
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