using System;
using System.Collections.Generic;
using Cringebot.Model;
using Cringebot.Wrappers;
using Foundation;
using UIKit;
using Cringebot.iOS;
using Cringebot.Services;
using System.Linq;
using UserNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppDelegate))]
namespace Cringebot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INotificationManager
    {
        public static Bootstrapper Bootstrapper;
        private static IEnumerable<Memory> _memories;
        private static Settings _settings;
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
            Forms.Init();

            Corcav.Behaviors.Infrastructure.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();

            Bootstrapper = new Bootstrapper();
            LoadApplication(Bootstrapper.ResolveApp());

            UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

            return base.FinishedLaunching(app, options);
        }

        public override void ApplicationSignificantTimeChange(UIApplication application)
        {
            if (_notificationsOn)
            {
                RestartNotificationQueue();
            }
        }

        public void StartNotifications(IEnumerable<Memory> memories, Settings settings)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
            }
            RestartNotificationQueue();
            _notificationsOn = true;
        }

        public void SetMemories(IEnumerable<Memory> memories)
        {
            _memories = memories;
            if (_notificationsOn)
            {
                RestartNotificationQueue();
            }
        }

        public void SetSettings(Settings settings)
        {
            _settings = settings;
            if (_notificationsOn)
            {
                RestartNotificationQueue();
            }
        }

        public void StopNotifications()
        {
            ClearExistingNotifications();
            _notificationsOn = false;
        }

        private static void ClearExistingNotifications()
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();
        }

        private static void RestartNotificationQueue()
        {
            ClearExistingNotifications();
            if (_memories == null || !_memories.Any()) return;
            if (_settings == null) return;

            const int IOS_NOTIFICATION_LIMIT = 64;
            double runningIntervalTotal = 0;
            for (var ii = 1; ii < IOS_NOTIFICATION_LIMIT-1; ii++)
            {
                runningIntervalTotal += NotificationRandomnessService.GetNotificationIntervalMilliseconds(_settings.GenerationMinInterval, _settings.GenerationMaxInterval);
                if (NotificationRandomnessService.DoesIntervalLandInDoNotDisturb(runningIntervalTotal, _settings.DoNotDisturbStartTime, _settings.DoNotDisturbStopTime))
                {
                    runningIntervalTotal += NotificationRandomnessService.GetDoNotDisturbLengthMilliseconds(_settings.DoNotDisturbStartTime, _settings.DoNotDisturbStopTime);
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

            var restartNotification = new UILocalNotification
            {
                FireDate = NSDate.FromTimeIntervalSinceNow(runningIntervalTotal / 1000),
                AlertTitle = "Notification limit reached",
                AlertBody = "Cringebot has reached the iOS-imposed limit of 64 scheduled notifications. Please relaunch Cringebot or toggle the simulation setting to receive notifications again.",
                SoundName = UILocalNotification.DefaultSoundName,
                RepeatInterval = NSCalendarUnit.Hour
            };
            UIApplication.SharedApplication.ScheduleLocalNotification(restartNotification);
        }
    }

    public class NotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}
