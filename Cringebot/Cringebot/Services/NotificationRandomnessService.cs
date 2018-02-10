using Cringebot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cringebot.Services
{
    public static class NotificationRandomnessService
    {
#if DEBUG
        private const int MAXIMUM_INTERVAL_MILLISECONDS = 1000 * 30;
        private const int MINIMUM_INTERVAL_MILLISECONDS = 1000 * 10;
#else
        private const int MAXIMUM_INTERVAL_MILLISECONDS = 1000 * 60 * 60 * 8;
        private const int MINIMUM_INTERVAL_MILLISECONDS = 1000 * 60 * 10;
#endif
        private static readonly TimeSpan _doNotDisturbStart = new TimeSpan(22,0,0);
        private static readonly TimeSpan _doNotDisturbStop = new TimeSpan(8, 0, 0);
        private static readonly int _doNotDisturbIntervalMilliseconds =
            (int) (_doNotDisturbStop.Add(new TimeSpan(1, 0, 0, 0)) - _doNotDisturbStart).TotalMilliseconds;

        private static readonly IEnumerable<string> _titleOptions = new[]
        {
            "Yikes",
            "Yeeesh",
            "Oof",
            "Oh boy",
            "Argh",
            "Sheesh",
            "Wow",
            "Oh gosh",
            "Man alive",
            "Jeez",
            "*Face palm*",
            "Good grief",
            "Why",
            "Ugh",
            "Gah",
            "Holy cow",
            "Oh man",
            "*Cringe*"
        };

        private static readonly Random _random;

        static NotificationRandomnessService()
        {
            _random = new Random();
        }

        public static int GetNotificationInterval()
        {
            var interval = _random.Next(MINIMUM_INTERVAL_MILLISECONDS, MAXIMUM_INTERVAL_MILLISECONDS);
            var notificationTime = DateTime.Now.AddMilliseconds(interval).TimeOfDay;
            if (notificationTime > _doNotDisturbStart ||
                notificationTime < _doNotDisturbStop)
            {
                return interval + _doNotDisturbIntervalMilliseconds;
            }
            return interval;
        }

        public static string GetNotificationTitle()
        {
            return _titleOptions.ElementAt(_random.Next(0, _titleOptions.Count())) + "..."; // random.next is max exclusive
        }

        public static Memory GetRandomMemory(IEnumerable<Memory> memory)
        {
            var memoryArray = memory as Memory[] ?? memory.ToArray();
            return memoryArray.ElementAt(_random.Next(0, memoryArray.Length)); // random.next is max exclusive
        }
    }
}
