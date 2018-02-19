using System;
using System.ComponentModel;

namespace Cringebot.Model
{
    public class Settings : INotifyPropertyChanged
    {
        public TimeSpan DoNotDisturbStartTime { get; set; }
        public TimeSpan DoNotDisturbStopTime { get; set; }
        public TimeSpan GenerationMaxInterval { get; set; }
        public TimeSpan GenerationMinInterval { get; set; }

        public Settings()
        {
            DoNotDisturbStartTime = new TimeSpan(22, 0, 0);
            DoNotDisturbStopTime = new TimeSpan(8, 0, 0);
            GenerationMaxInterval = new TimeSpan(8, 0, 0);
            GenerationMinInterval = new TimeSpan(1, 0, 0);
            //GenerationMaxInterval = new TimeSpan(0, 0, 30);
            //GenerationMinInterval = new TimeSpan(0, 0, 10);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}