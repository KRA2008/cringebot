using System;

namespace Cringebot.Wrappers
{
    /// http://ayende.com/blog/3408/dealing-with-time-in-tests
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;

        static SystemTime()
        {
            ResetToDefault();
        }

        public static void ResetToDefault()
        {
            Now = () => DateTime.Now;
            UtcNow = () => DateTime.UtcNow;
        }
    }
}
