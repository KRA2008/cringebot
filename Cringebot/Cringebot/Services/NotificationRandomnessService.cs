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
            return _random.Next(MINIMUM_INTERVAL_MILLISECONDS, MAXIMUM_INTERVAL_MILLISECONDS);
        }

        public static string GetNotificationTitle()
        {
            return _titleOptions.ElementAt(_random.Next(0, _titleOptions.Count() - 1)) + "...";
        }

        public static Memory GetRandomMemory(IEnumerable<Memory> memory)
        {
            var memoryArray = memory as Memory[] ?? memory.ToArray();
            return memoryArray.ElementAt(_random.Next(0, memoryArray.Length - 1));
        }
    }
}
