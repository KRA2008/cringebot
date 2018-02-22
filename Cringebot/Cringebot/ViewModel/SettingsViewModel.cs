using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cringebot.Model;
using Cringebot.Wrappers;
using FreshMvvm;

namespace Cringebot.ViewModel
{
    public class SettingsViewModel : FreshBasePageModel
    {
        private const double RAPID_FIRE_TIME_HOURS = 0.005;
        private const double MAX_HOURS = 999;

        private readonly INotificationManager _notificationManager;
        public Settings Settings { get; set; }

        public IList<double> MaxHoursChoices { get; set; }
        public IList<double> MinHoursChoices { get; set; }
        public double MaxHours { get; set; }
        public double MinHours { get; set; }

        public string DoNotDisturbExplanation => "You will receive no notifications between " + DateTime.Today.Add(Settings.DoNotDisturbStartTime).ToString("t") +
                                                 " and " + DateTime.Today.Add(Settings.DoNotDisturbStopTime).ToString("t") + ".";
        public string GenerationIntervalExplanation => "Notifications will be generated at random intervals between " + MinHours +
                                                       " and " + MaxHours + " hours in length.";

        public SettingsViewModel(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
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
        }

        public void SaveSettings()
        {
            Settings.GenerationMaxInterval = new TimeSpan((int)MaxHours / 24, (int)MaxHours % 24, (int)(MaxHours % 1 * 60), (int)(MaxHours % 1 * 60 % 1 * 60));
            Settings.GenerationMinInterval = new TimeSpan((int)MinHours / 24, (int)MinHours % 24, 0, 0);
            _notificationManager.SetSettings(Settings);
        }
    }
}