using Android.App;
using Android.Content;
using Cringebot.Wrappers;
using Android.OS;
using Cringebot.Model;
using Cringebot.Services;
using Application = Android.App.Application;
using AndroidX.Core.App;

namespace Cringebot.Droid.PlatformSpecific
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class MyNotificationManager : BroadcastReceiver, INotificationManager
    {
        public const int NOTIFICATION_REQUEST_CODE = 5;
        public const string NOTIFICATION_TITLE_EXTRA = "notificationTitle";
        public const string NOTIFICATION_TEXT_EXTRA = "notificationText";

        public const string CHANNEL_ID = "default";

        NotificationManagerCompat compatManager;

        private static IEnumerable<Memory> _memories;
        private static Settings _settings;
        private bool _notificationsOn;
        private static bool channelInitialized = false;

        public static MyNotificationManager Instance { get; private set; }

        public MyNotificationManager()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                compatManager = NotificationManagerCompat.From(Platform.AppContext);
                Instance = this;
            }
        }

        public void StopNotifications()
        {
            CancelNextNotification();
            _notificationsOn = false;
        }

        public void StartNotifications(IEnumerable<Memory> memories, Settings settings)
        {
            _memories = memories;
            _settings = settings;
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

        public override void OnReceive(Context context, Intent intent)
        {
            if(_memories != null && _memories.Any())
            {
                var createNotificationIntent = new Intent(context, typeof(NotificationCreationService));
                createNotificationIntent.PutExtra(NOTIFICATION_TITLE_EXTRA, NotificationRandomnessService.GetNotificationTitle());
                createNotificationIntent.PutExtra(NOTIFICATION_TEXT_EXTRA, NotificationRandomnessService.GetRandomMemory(_memories).Description);
                context.StartService(createNotificationIntent);
            }
            SetNextNotification();
        }

        private static void RestartNotificationQueue()
        {
            CancelNextNotification();
            SetNextNotification();
        }

        private static void SetNextNotification()
        {
            if (_settings == null) return;

            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            var timerIntent = new Intent(Application.Context, typeof(MyNotificationManager));
            var timerPendingIntent = PendingIntent.GetBroadcast
            (Application.Context, NOTIFICATION_REQUEST_CODE, timerIntent,
                PendingIntentFlags.UpdateCurrent |
                (Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Immutable : 0));
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var notificationInterval = NotificationRandomnessService.GetNotificationIntervalMilliseconds(_settings.GenerationMinInterval, _settings.GenerationMaxInterval);
            if (NotificationRandomnessService.DoesIntervalLandInDoNotDisturb(notificationInterval, _settings.DoNotDisturbStartTime, _settings.DoNotDisturbStopTime))
            {
                notificationInterval += NotificationRandomnessService.GetDoNotDisturbLengthMilliseconds(_settings.DoNotDisturbStartTime, _settings.DoNotDisturbStopTime);
            }
            var alarmTimeMillis = SystemClock.ElapsedRealtime() + notificationInterval;
            alarmManager.SetExact(AlarmType.ElapsedRealtime, alarmTimeMillis, timerPendingIntent);
        }

        private static void CancelNextNotification()
        {
            var cancelTimerIntent = new Intent(Application.Context, typeof(MyNotificationManager));
            var cancelTimerPendingIntent = PendingIntent.GetBroadcast(Application.Context, NOTIFICATION_REQUEST_CODE,
                cancelTimerIntent,
                Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Immutable : 0);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(cancelTimerPendingIntent);
        }

        private static void CreateNotificationChannel()
        {
            // Create the notification channel, but only on API 26+.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String("Default");
                var channel = new NotificationChannel(CHANNEL_ID, channelNameJava, NotificationImportance.Default)
                {
                    Description = "Default channel for Cringebot notifications."
                };
                // Register the channel
                var manager = (NotificationManager)Platform.AppContext.GetSystemService(Context.NotificationService);
                manager.CreateNotificationChannel(channel);
                channelInitialized = true;
            }
        }
    }
}