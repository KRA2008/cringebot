﻿using Cringebot.iOS.CustomRenderers;
using Cringebot.Services;
using Cringebot.ViewModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Picker), typeof(ThemedPickerRenderer))]
namespace Cringebot.iOS.CustomRenderers
{
    public class ThemedPickerRenderer : PickerRenderer
    {
        private bool _initialized;

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && !_initialized)
            {
                ApplyTheme(null);
                MessagingCenter.Subscribe<SettingsViewModel>(this, ThemeService.THEME_SET_MESSAGE, ApplyTheme);
                MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE, Unsubscribe);
                _initialized = true;
            }
        }

        private void Unsubscribe(SettingsViewModel vm)
        {
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, ThemeService.THEME_SET_MESSAGE);
            MessagingCenter.Unsubscribe<SettingsViewModel>(this, SettingsViewModel.DESTROY_SETTINGS_MESSAGE);
        }

        private void ApplyTheme(SettingsViewModel vm)
        {
            var font = Xamarin.Forms.Application.Current.Resources["styledFontShort"];
            if (font != null)
            {
                Control.Font = UIFont.FromName((string)font, 16);
            }
            var backgroundColor = Xamarin.Forms.Application.Current.Resources["styledPageBackgroundColor"];
            if (backgroundColor != null)
            {
                var color = (Color) backgroundColor;
                Control.BackgroundColor = UIColor.FromRGB((int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
            }
        }
    }
}