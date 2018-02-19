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
        private const decimal RAPID_FIRE_TIME_HOURS = 0.02m;

        private readonly INotificationManager _notificationManager;
        public Settings Settings { get; set; }

        public IList<decimal> MaxHoursChoices { get; set; }
        public IList<decimal> MinHoursChoices { get; set; }
        public decimal MaxHours { get; set; }
        public decimal MinHours { get; set; }

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

            MinHoursChoices = new List<decimal>{0};
            MaxHoursChoices = new List<decimal>{ RAPID_FIRE_TIME_HOURS };
            for (var i = 1; i < 100; i++)
            {
                MaxHoursChoices.Add(i);
                MinHoursChoices.Add(i);
            }

            MaxHours = Math.Round(GetHoursFromTimeSpan(Settings.GenerationMaxInterval),2);
            MinHours = GetHoursFromTimeSpan(Settings.GenerationMinInterval);
        }

        private static decimal GetHoursFromTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.Days * 24 +
                   timeSpan.Hours +
                   timeSpan.Minutes / 60m;
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
            Settings.GenerationMaxInterval = new TimeSpan((int)MaxHours / 24, (int)MaxHours % 24, (int)(MaxHours % 1 * 60), 0);
            Settings.GenerationMinInterval = new TimeSpan((int)MinHours / 24, (int)MinHours % 24, 0, 0);
            _notificationManager.SetSettings(Settings);
        }
    }
}