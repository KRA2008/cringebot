using System.Collections.Generic;
using Cringebot.Model;
using Cringebot.Wrappers;
using Foundation;
using UIKit;
using Cringebot.iOS;
using Cringebot.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AppDelegate))]
namespace Cringebot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INotificationManager
    {
        private static IEnumerable<Memory> _memories;

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

            HandleLaunchNotification(options);

            return base.FinishedLaunching(app, options);
        }

        private void HandleLaunchNotification(NSDictionary launchOptions)
        {
            if (launchOptions != null)
            {
                if (launchOptions.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    if (launchOptions[UIApplication.LaunchOptionsLocalNotificationKey] is UILocalNotification localNotification)
                    {
                        UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        Window.RootViewController.PresentViewController(okayAlertController, true, null);

                        CreateNotification();
                    }
                }
            }
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            var alert = new UIAlertView(notification.AlertTitle, notification.AlertBody, null, "OK");
            alert.Show();

            CreateNotification();
        }

        public void ActivateNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
            }
            CreateNotification();
        }

        public void CancelNotifications()
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();
        }

        public void SetMemories(IEnumerable<Memory> memories)
        {
            _memories = memories;
        }

        private void CreateNotification()
        {
            var fireDate = NSDate.FromTimeIntervalSinceNow(NotificationRandomnessService.GetNotificationInterval() / 1000);
            var notification = new UILocalNotification
            {
                FireDate = fireDate,
                AlertTitle = NotificationRandomnessService.GetNotificationTitle(),
                AlertBody = NotificationRandomnessService.GetRandomMemory(_memories).Description,
                SoundName = UILocalNotification.DefaultSoundName
            };

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
