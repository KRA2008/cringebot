using Android.App;
using Android.Content;
using Android.Support.V4.App;

namespace Cringebot.Droid.PlatformSpecific
{
    [Service]
    public class NotificationCreationService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var builder = new NotificationCompat.Builder(Application.Context)
                .SetDefaults(NotificationCompat.DefaultAll)
                .SetContentTitle(intent.GetStringExtra(MyNotificationManager.NOTIFICATION_TITLE_EXTRA))
                .SetContentText(intent.GetStringExtra(MyNotificationManager.NOTIFICATION_TEXT_EXTRA))
                .SetSmallIcon(Resource.Drawable.icon);
            
            var notification = builder.Build();
            
            var notificationManager = Application.Context.GetSystemService(NotificationService) as NotificationManager;
            
            const int NOTIFICATION_ID = 0;
            notificationManager?.Notify(NOTIFICATION_ID, notification);
        }
    }
}