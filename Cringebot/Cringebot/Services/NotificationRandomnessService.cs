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
        private static readonly IEnumerable<string> TITLE_OPTIONS = new[]
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

        private static Random _random;

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
            return TITLE_OPTIONS.ElementAt(_random.Next(0, TITLE_OPTIONS.Count() - 1)) + "...";
        }

        public static Memory GetRandomMemory(IEnumerable<Memory> memory)
        {
            return memory.ElementAt(_random.Next(0, memory.Count() - 1));
        }
    }
}
