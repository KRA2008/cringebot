using System.Collections.Generic;
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
        private string _currentThemeName;

        public ThemeService(IDeviceWrapper deviceWrapper)
        {
            _deviceWrapper = deviceWrapper;
            _themes = new[]
            {
                new Theme
                {
                    Name = "Cringe",
                    AndroidFontLong = "Comic Sans MS.ttf#Comic Sans MS",
                    AndroidFontShort = "Comic Sans MS.ttf",
                    iOSFont = "Comic Sans MS",
                    TextColor = Color.Black,
                    PlaceholderColor = Color.DimGray,
                    NavBarColor = Color.Red,
                    NavBarTextColor = Color.Green,
                    PageBackgroundColor = Color.Turquoise,
                    ButtonBackgroundColor = Color.Yellow,
                    ButtonTextColor = Color.DeepPink,
                    ButtonCornerRadius = 15
                },
                new Theme
                {
                    Name = "Mac & Cheese",
                    iOSFont = "SBC Macaroni Regular",
                    AndroidFontLong = "SBC Macaroni Regular.ttf#SBC Macaroni Regular",
                    AndroidFontShort = "SBC Macaroni Regular.ttf",
                    TextColor = Color.Orange,
                    ButtonTextColor = Color.Orange,
                    PageBackgroundColor = Color.Yellow,
                    ButtonBackgroundColor = Color.OrangeRed
                },
                new Theme
                {
                    Name = "Goth",
                    TextColor = Color.DarkRed,
                    ButtonTextColor = Color.DarkRed,
                    PageBackgroundColor = Color.Black,
                    ButtonBackgroundColor = Color.DimGray
                }
            };
        }

        public void ApplyTheme(string name)
        {
            var targetTheme = _themes.FirstOrDefault(t => t.Name == name) ?? _themes.First();

            _currentThemeName = targetTheme.Name;
            var isiOS = _deviceWrapper.RuntimePlatform() == Device.iOS;

            Application.Current.Resources["styledTextColor"] = targetTheme.TextColor;
            Application.Current.Resources["styledButtonTextColor"] = targetTheme.ButtonTextColor;
            Application.Current.Resources["styledPageBackgroundColor"] = targetTheme.PageBackgroundColor;
            Application.Current.Resources["styledButtonBackgroundColor"] = targetTheme.ButtonBackgroundColor;
            Application.Current.Resources["styledFontShort"] = isiOS ? targetTheme.iOSFont : targetTheme.AndroidFontShort;
            Application.Current.Resources["styledFontLong"] = isiOS ? targetTheme.iOSFont : targetTheme.AndroidFontLong;

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