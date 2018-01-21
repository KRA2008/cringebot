﻿using Android.App;
using Android.Content;
using Cringebot.Wrappers;
using Cringebot.Droid.PlatformSpecific;
using Android.OS;
using Cringebot.Model;
using System.Collections.Generic;
using Cringebot.Services;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(MyNotificationManager))]
namespace Cringebot.Droid.PlatformSpecific
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class MyNotificationManager : BroadcastReceiver, INotificationManager
    {
        public const int NOTIFICATION_REQUEST_CODE = 5;
        public const string NOTIFICATION_TITLE_EXTRA = "notificationTitle";
        public const string NOTIFICATION_TEXT_EXTRA = "notificationText";

        private static IEnumerable<Memory> _memories;

        public void StopNotifications()
        {
            CancelNextNotification();
        }

        public void StartNotifications()
        {
            SetNextNotification();
        }

        public void SetMemories(IEnumerable<Memory> memories)
        {
            _memories = memories;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if(_memories != null && _memories.Count() > 0)
            {
                Intent createNotificationIntent = new Intent(context, typeof(NotificationCreationService));
                createNotificationIntent.PutExtra(NOTIFICATION_TITLE_EXTRA, NotificationRandomnessService.GetNotificationTitle());
                createNotificationIntent.PutExtra(NOTIFICATION_TEXT_EXTRA, NotificationRandomnessService.GetRandomMemory(_memories).Description);
                context.StartService(createNotificationIntent);
            }
            SetNextNotification();
        }

        private void SetNextNotification()
        {
            var timerIntent = new Intent(Application.Context, typeof(MyNotificationManager));
            var timerPendingIntent = PendingIntent.GetBroadcast
                (Application.Context, NOTIFICATION_REQUEST_CODE, timerIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var alarmTimeMillis = SystemClock.ElapsedRealtime() + NotificationRandomnessService.GetNotificationInterval();
            alarmManager.SetExact(AlarmType.ElapsedRealtime, alarmTimeMillis, timerPendingIntent);
        }

        private void CancelNextNotification()
        {
            var cancelTimerIntent = new Intent(Application.Context, typeof(MyNotificationManager));
            var cancelTimerPendingIntent = PendingIntent.GetBroadcast(Application.Context, NOTIFICATION_REQUEST_CODE, cancelTimerIntent, 0); //why 0?
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(cancelTimerPendingIntent);
        }
    }
}