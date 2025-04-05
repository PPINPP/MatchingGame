#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    void Start() {
        CreateNotificationChannel();
        ScheduleExitNotification();
    }
    void CreateNotificationChannel() {
        var channel = new AndroidNotificationChannel() {
            Id = "exit_channel",
            Name = "Exit Notifications",
            Importance = Importance.Default,
            Description = "Notifies when the app is closed",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    void ScheduleExitNotification() {
        // DateTime fireTime = DateTime.Now.AddSeconds(10);
        DateTime fireTime = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
        if(DateTime.Now > fireTime){
            fireTime = fireTime.AddDays(1);
        }

        var notification = new AndroidNotification {
            Title = "Come Back!",
            Text = "It's been a minute since you left. We miss you!",
            SmallIcon = "icon_small",
            FireTime = fireTime,
        };

        AndroidNotificationCenter.SendNotification(notification, "exit_channel");
    }

}

#endif