using System.Collections.Generic;
using Cringebot.Model;
using Cringebot.Wrappers;
using Foundation;
using UIKit;
using Cringebot.iOS;
using Cringebot.Services;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(AppDelegate))]
namespace Cringebot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INotificationManager
    {
        private static IEnumerable<Memory> _memories;
        private static bool _notificationsOn;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();

            Corcav.Behaviors.Infrastructure.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            RestartNotificationQueue();
        }

        public void StartNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
            }
            _notificationsOn = true;
            RestartNotificationQueue();
        }

        public void StopNotifications()
        {
            _notificationsOn = false;
            ClearExistingNotifications();
        }

        public void SetMemories(IEnumerable<Memory> memories)
        {
            _memories = memories;
            if(_notificationsOn)
            {
                RestartNotificationQueue();
            }
        }

        private static void ClearExistingNotifications()
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();
        }

        private static void RestartNotificationQueue()
        {
            if (_memories == null || !_memories.Any()) return;

            ClearExistingNotifications();
            const int IOS_NOTIFICATION_LIMIT = 64;
            double runningIntervalTotal = 0;
            for (var ii = 1; ii < IOS_NOTIFICATION_LIMIT; ii++)
            {
                runningIntervalTotal += NotificationRandomnessService.GetNotificationIntervalMilliseconds();
                if (NotificationRandomnessService.DoesIntervalLandInDoNotDisturb(runningIntervalTotal))
                {
                    runningIntervalTotal += NotificationRandomnessService.DoNotDisturbLengthMilliseconds;
                }
                var notification = new UILocalNotification
                {
                    FireDate = NSDate.FromTimeIntervalSinceNow(runningIntervalTotal / 1000),
                    AlertTitle = NotificationRandomnessService.GetNotificationTitle(),
                    AlertBody = NotificationRandomnessService.GetRandomMemory(_memories).Description,
                    SoundName = UILocalNotification.DefaultSoundName
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }
    }
}
