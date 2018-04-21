﻿using System.Collections.Generic;
using System.Linq;
using Cringebot.Model;
using Cringebot.Wrappers;
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
        private static string _currentThemeName; // I have this registered as singleton but it appears to be instantiating multiple times anyway

        public ThemeService(IDeviceWrapper deviceWrapper)
        {
            _deviceWrapper = deviceWrapper;
            _themes = new[]
            {
                new Theme
                {
                    Name = "Cringe",
                    Font = "Comic Sans MS",
                    TextColor = Color.Black,
                    PlaceholderColor = Color.DimGray,
                    NavBarColor = Color.Red,
                    NavBarTextColor = Color.Green,
                    PageBackgroundColor = Color.Turquoise,
                    ButtonBackgroundColor = Color.Yellow,
                    ButtonTextColor = Color.DeepPink,
                    ButtonCornerRadius = 10
                },
                new Theme
                {
                    Name = "Goth",
                    Font = "Plain Black",
                    TextColor = Color.White,
                    ButtonTextColor = Color.Red,
                    PageBackgroundColor = Color.Black,
                    ButtonBackgroundColor = Color.White,
                    PlaceholderColor = Color.Red,
                    NavBarColor = Color.DarkRed,
                    NavBarTextColor = Color.Black,
                    ButtonCornerRadius = 0
                },
                new Theme
                {
                    Name = "Emo",
                    Font = "Emo",
                    TextColor = Color.DarkRed,
                    ButtonTextColor = Color.DarkRed,
                    PageBackgroundColor = Color.Black,
                    ButtonBackgroundColor = Color.White,
                    PlaceholderColor = Color.Red,
                    NavBarColor = Color.DarkRed,
                    NavBarTextColor = Color.Black,
                    ButtonCornerRadius = 0
                },
                new Theme
                {
                    Name = "Mac & Cheese",
                    Font = "SBC Macaroni Regular",
                    TextColor = Color.OrangeRed,
                    ButtonTextColor = Color.Yellow,
                    PageBackgroundColor = Color.Yellow,
                    ButtonBackgroundColor = Color.Orange,
                    NavBarColor = Color.Orange,
                    NavBarTextColor = Color.Yellow,
                    PlaceholderColor = Color.Orange,
                    ButtonCornerRadius = 25
                }
            };
        }

        public void ApplyTheme(string name)
        {
            var targetTheme = _themes.FirstOrDefault(t => t.Name == name) ?? _themes.First();

            _currentThemeName = targetTheme.Name;
            var isiOS = _deviceWrapper.RuntimePlatform() == Device.iOS;

            Application.Current.Resources["styledTextColor"] = targetTheme.TextColor;
            Application.Current.Resources["styledPageBackgroundColor"] = targetTheme.PageBackgroundColor;
            Application.Current.Resources["styledButtonCornerRadius"] = targetTheme.ButtonCornerRadius;
            Application.Current.Resources["styledButtonBackgroundColor"] = targetTheme.ButtonBackgroundColor;
            Application.Current.Resources["styledButtonTextColor"] = targetTheme.ButtonTextColor;
            Application.Current.Resources["styledPlaceholderColor"] = targetTheme.PlaceholderColor;
            Application.Current.Resources["styledNavBarColor"] = targetTheme.NavBarColor;
            Application.Current.Resources["styledNavBarTextColor"] = targetTheme.NavBarTextColor;
            Application.Current.Resources["styledFontShort"] = isiOS ? targetTheme.Font : targetTheme.Font+".ttf";
            Application.Current.Resources["styledFontLong"] = isiOS ? targetTheme.Font : targetTheme.Font + ".ttf#" + targetTheme.Font;

            MessagingCenter.Send(this, THEME_SET_MESSAGE);
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