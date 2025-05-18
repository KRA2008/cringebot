using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Application = Android.App.Application;
using Resource = Microsoft.Maui.Resource;

namespace Cringebot.Droid.PlatformSpecific
{
    [Service]
    public class NotificationCreationService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var builder = new NotificationCompat.Builder(Application.Context, MyNotificationManager.CHANNEL_ID)
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