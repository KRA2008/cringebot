using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cringebot.Model;
using Cringebot.Services;
using Cringebot.Wrappers;
using FreshMvvm;
using Xamarin.Forms;

namespace Cringebot.ViewModel
{
    public class SettingsViewModel : FreshBasePageModel
    {
        public const string DESTROY_SETTINGS_MESSAGE = "destroySettings";

        private const double RAPID_FIRE_TIME_HOURS = 0.005;
        private const double MAX_HOURS = 999;
        public Settings Settings { get; set; }

        public IList<double> MaxHoursChoices { get; set; }
        public IList<double> MinHoursChoices { get; set; }
        public double MaxHours { get; set; }
        public double MinHours { get; set; }

        public ObservableCollection<string> ThemeNames { get; set; }

        public string DoNotDisturbExplanation => "You will receive no notifications between " + DateTime.Today.Add(Settings.DoNotDisturbStartTime).ToString("t") +
                                                 " and " + DateTime.Today.Add(Settings.DoNotDisturbStopTime).ToString("t") + ".";
        public string GenerationIntervalExplanation => "Notifications will be generated at random intervals between " + MinHours +
                                                       " and " + MaxHours + " hours in length.";

        public Command SetTheme { get; set; }
        private readonly INotificationManager _notificationManager;
        private readonly IThemeService _themeService;

        public SettingsViewModel(INotificationManager notificationManager, IThemeService themeService)
        {
            _notificationManager = notificationManager;
            _themeService = themeService;
            SetTheme = new Command(SetThemeMethod);
        }

        private void SetThemeMethod(object obj)
        {
            var themeName = (string)obj;
            _themeService.ApplyTheme(themeName);
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            Settings = (Settings) initData;
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            PropertyChanged += OnPropertyChanged;

            MinHoursChoices = new List<double> {0};
            MaxHoursChoices = new List<double> { RAPID_FIRE_TIME_HOURS };
            for (var i = 1; i < MAX_HOURS+1; i++)
            {
                MaxHoursChoices.Add(i);
                MinHoursChoices.Add(i);
            }

            MaxHours = GetHoursFromTimeSpan(Settings.GenerationMaxInterval);
            MinHours = GetHoursFromTimeSpan(Settings.GenerationMinInterval);

            ThemeNames = new ObservableCollection<string>(_themeService.GetThemes());
        }

        private static double GetHoursFromTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.Days * 24 +
                   timeSpan.Hours +
                   timeSpan.Minutes / 60.0 +
                   timeSpan.Seconds / (60.0 * 60.0);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(MaxHours))
            {
                if (MaxHours < MinHours)
                {
                    MaxHours = MinHours;
                    RaisePropertyChanged(nameof(MaxHours));
                }
                RaisePropertyChanged(nameof(GenerationIntervalExplanation));
            }

            if (args.PropertyName == nameof(MinHours))
            {
                if (MinHours > MaxHours)
                {
                    if (MaxHours == RAPID_FIRE_TIME_HOURS)
                    {
                        MinHours = 0;
                    }
                    else
                    {
                        MinHours = MaxHours;
                    }
                    RaisePropertyChanged(nameof(MinHours));
                }
                RaisePropertyChanged(nameof(GenerationIntervalExplanation));
            }
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Settings.DoNotDisturbStartTime) ||
                args.PropertyName == nameof(Settings.DoNotDisturbStopTime))
            {
                RaisePropertyChanged(nameof(DoNotDisturbExplanation));
            }
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
            SaveSettings();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
            PropertyChanged -= OnPropertyChanged;
            MessagingCenter.Send(this, DESTROY_SETTINGS_MESSAGE);
        }

        public void SaveSettings()
        {
            Settings.GenerationMaxInterval = new TimeSpan((int)MaxHours / 24, (int)MaxHours % 24, (int)(MaxHours % 1 * 60), (int)(MaxHours % 1 * 60 % 1 * 60));
            Settings.GenerationMinInterval = new TimeSpan((int)MinHours / 24, (int)MinHours % 24, 0, 0);
            _notificationManager.SetSettings(Settings);
        }
    }
}