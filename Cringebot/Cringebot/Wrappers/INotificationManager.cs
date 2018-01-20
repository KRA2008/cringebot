using Cringebot.Model;
using System.Collections.Generic;

namespace Cringebot.Wrappers
{
    public interface INotificationManager
    {
        void ActivateNotifications();
        void CancelNotifications();
        void SetMemories(IEnumerable<Memory> memories);
    }
}
