using Cringebot.Model;
using System.Collections.Generic;

namespace Cringebot.Wrappers
{
    public interface INotificationManager
    {
        void StartNotifications();
        void StopNotifications();
        void SetMemories(IEnumerable<Memory> memories);
        void SetSettings(Settings settings);
    }
}
