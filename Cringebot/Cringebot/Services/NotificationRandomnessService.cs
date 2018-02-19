using Cringebot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cringebot.Services
{
    public static class NotificationRandomnessService
    {
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

        public static bool DoesIntervalLandInDoNotDisturb(double originalIntervalMilliseconds, TimeSpan doNotDisturbStart, TimeSpan doNotDisturbStop)
        {
            var notificationTime = DateTime.Now.AddMilliseconds(originalIntervalMilliseconds).TimeOfDay;
            
            //https://stackoverflow.com/a/21343435
            if (doNotDisturbStart <= doNotDisturbStop)
            {
                // start and stop times are in the same day
                if (notificationTime >= doNotDisturbStart && notificationTime <= doNotDisturbStop)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            else
            {
                // start and stop times are in different days
                if (notificationTime >= doNotDisturbStart || notificationTime <= doNotDisturbStop)
                {
                    // current time is between start and stop
                    return true;
                }
            }
            return false;
        }

        public static int GetNotificationIntervalMilliseconds(TimeSpan generationMinimumInterval, TimeSpan generationMaximumInterval)
        {
            return _random.Next((int)generationMinimumInterval.TotalMilliseconds, (int)generationMaximumInterval.TotalMilliseconds);
        }

        public static int GetDoNotDisturbLengthMilliseconds(TimeSpan doNotDisturbStart, TimeSpan doNotDisturbStop)
        {
            return (int)(doNotDisturbStop.Add(new TimeSpan(1, 0, 0, 0)) - doNotDisturbStart).TotalMilliseconds;
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
