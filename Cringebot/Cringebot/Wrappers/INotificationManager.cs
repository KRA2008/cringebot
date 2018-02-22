using Cringebot.Model;
using System.Collections.Generic;

namespace Cringebot.Wrappers
{
    public interface INotificationManager
    {
        void StartNotifications(IEnumerable<Memory> memories, Settings settings);
        void StopNotifications();
        void SetMemories(IEnumerable<Memory> memories);
        void SetSettings(Settings settings);
    }
}
